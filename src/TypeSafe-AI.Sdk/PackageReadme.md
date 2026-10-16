# TypeSafe-AI.Sdk

## Examples

### Register
``` csharp
var apiKey = Environment.GetEnvironmentVariable("TYPESAFE_API_KEY") ?? string.Empty;

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Please set TYPESAFE_API_KEY before running the example.");
    return;
}

var options = new JsonSerializerOptions { WriteIndented = true };

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTypeSafeSdk(options =>
{
    options.ApiKey = apiKey;
});

using var host = builder.Build();

var client = host.Services.GetRequiredService<ITypeSafeClient>();
```

### Create a Noul request
``` csharp
var requestNoul = new EvaluateRequest
{
    State = "Help! My payouts have been failing for 3 days.",
    Model = "jev-latest",
    Questions = new Dictionary<string, Question>
    {
        ["is_urgent"] = Question.Noul(
            "Does this convey urgency?",
            yesCriteria: "Explicitly time-sensitive",
            noCriteria: "No urgency expressed"),
    },
};

var responseNoul = await client.EvaluateAsync(requestNoul);

Console.WriteLine($"State: {requestNoul.State}");

if (responseNoul.Answers.TryGetValue("is_urgent", out var answerNoul))
{
    Console.WriteLine($"Type:  {answerNoul.Type}");
    Console.WriteLine($"Noul:  {answerNoul.Noul:0.###}");
}

Console.WriteLine($"Model: {responseNoul.Model}");
Console.WriteLine($"Usage: InputTokens={responseNoul.Usage.InputTokens}, OutputTokens={responseNoul.Usage.OutputTokens}");
```

### Create a Choice request
``` csharp
var requestChoice = new EvaluateRequest
{
    State = "My running shoes arrived in the wrong size. Can I swap them for a size 10?",
    Model = "jev-latest",
    Questions = new Dictionary<string, Question>
    {
        ["instructions"] = Question.Choice(
            "Which team should handle this?",
            criteria: new Dictionary<string, object?>
            {
                ["returns"] = "Exchanges, wrong or damaged items",
                ["shipping"] = "Delivery status, delays, lost packages",
                ["billing"] = "Charges, invoices, payment problems"
            })
    },
};

var responseChoice = await client.EvaluateAsync(requestChoice);

Console.WriteLine($"State:         {requestChoice.State}");

if (responseChoice.Answers.TryGetValue("instructions", out var answerChoice))
{
    Console.WriteLine($"Type:          {answerChoice.Type}");
    Console.WriteLine($"Choice:        {answerChoice.Choice}");
    Console.WriteLine($"Confidence:    {answerChoice.Confidence:0.###}");
    Console.WriteLine($"Probabilities: {JsonSerializer.Serialize(answerChoice.Probabilities, options)}");
}

Console.WriteLine($"Model:          {responseChoice.Model}");
Console.WriteLine($"Usage:          InputTokens={responseChoice.Usage.InputTokens}, OutputTokens={responseChoice.Usage.OutputTokens}");
```

### Create a Score request
``` csharp
var requestScore = new EvaluateRequest
{
    State = "The export button crashes the settings page in Safari. It works in Chrome, but a few of our customers only use Safari.",
    Model = "jev-latest",
    Questions = new Dictionary<string, Question>
    {
        ["bug_severity"] = Question.Score(
            "How severe is the reported issue?",
            criteria:
            [
                "Cosmetic; no impact to functionality",
                "Broken or degraded feature, but workaround exists",
                "Blocking issue; no workaround exists"
            ])
    },
};

var responseScore = await client.EvaluateAsync(requestScore);

Console.WriteLine($"State:         {requestScore.State}");

if (responseScore.Answers.TryGetValue("bug_severity", out var answerScore))
{
    Console.WriteLine($"Type:          {answerScore.Type}");
    Console.WriteLine($"Score:         {answerScore.Score}");
    Console.WriteLine($"Confidence:    {answerScore.Confidence:0.###}");
    Console.WriteLine($"Legend:        {JsonSerializer.Serialize(answerScore.Legend, options)}");
    Console.WriteLine($"Probabilities: {JsonSerializer.Serialize(answerScore.Probabilities, options)}");
}

Console.WriteLine($"Model:         {responseScore.Model}");
Console.WriteLine($"Usage:         InputTokens={responseScore.Usage.InputTokens}, OutputTokens={responseScore.Usage.OutputTokens}");
```

### Create a Box request
``` csharp
builder.Services.AddTypeSafeSdk(options =>
{
    options.ApiKey = apiKey;
    options.EnableImagesDangerZone = true;
});

var imageBytes = await File.ReadAllBytesAsync("mug.jpg");
var imageBase64 = Convert.ToBase64String(imageBytes);

var requestBox = new EvaluateRequest
{
    Model = "jev-preview",
    State = new
    {
        photo = "mug-photo"
    },
    Assets = new Dictionary<string, Asset>
    {
        ["mug-photo"] = new Asset
        {
            Type = "image",
            MimeType = "image/jpeg",
            Base64 = imageBase64
        }
    },
    Questions = new Dictionary<string, Question>
    {
        ["mug"] = Question.Box("the blue espresso mug in `photo`")
    }
};

var responseBox = await client.EvaluateAsync(requestBox);

if (responseBox.Answers.TryGetValue("mug", out var answerBox) && answerBox.Box is not null)
{
    Console.WriteLine($"Top: {answerBox.Box.Top:0.###}, Bottom: {answerBox.Box.Bottom:0.###}, Left: {answerBox.Box.Left:0.###}, Right: {answerBox.Box.Right:0.###}");
    Console.WriteLine($"Confidence: {answerBox.Confidence:0.###}");
}
```

### Create a Box request with multipart upload
``` csharp
builder.Services.AddTypeSafeSdk(options =>
{
    options.ApiKey = apiKey;
    options.EnableImagesDangerZone = true;
});

await using var imageStream = File.OpenRead("mug.jpg");

var requestBoxMultipart = new EvaluateRequest
{
    Model = "jev-preview",
    State = new
    {
        photo = "mug-photo"
    },
    Assets = new Dictionary<string, Asset>
    {
        ["mug-photo"] = new Asset
        {
            Type = "image",
            Part = "mug-photo-part"
        }
    },
    Questions = new Dictionary<string, Question>
    {
        ["mug"] = Question.Box("the blue espresso mug in `photo`")
    }
};

var responseBoxMultipart = await client.EvaluateBoxAsync(requestBoxMultipart, "mug-photo-part", imageStream, "mug.jpg", "image/jpeg");

if (responseBoxMultipart.Answers.TryGetValue("mug", out var answerBoxMultipart) && answerBoxMultipart.Box is not null)
{
    Console.WriteLine($"Top: {answerBoxMultipart.Box.Top:0.###}, Bottom: {answerBoxMultipart.Box.Bottom:0.###}, Left: {answerBoxMultipart.Box.Left:0.###}, Right: {answerBoxMultipart.Box.Right:0.###}");
    Console.WriteLine($"Confidence: {answerBoxMultipart.Confidence:0.###}");
}
```

### Create an image request with multipart upload (dog example)
``` csharp
await using var foundDogImageStream = File.OpenRead("dog-found.jpg");
await using var dogPosterImageStream = File.OpenRead("dog-poster.png");

var requestImagesMultipart = new EvaluateRequest
{
    Model = "jev-latest",
    State = new
    {
        photo = "a3f1c2e8-found"
    },
    Assets = new Dictionary<string, Asset>
    {
        ["a3f1c2e8-found"] = new Asset
        {
            Type = "image",
            Part = "dog-found-photo"
        },
        ["77b0d4a1-poster"] = new Asset
        {
            Type = "image",
            Part = "dog-poster-photo"
        }
    },
    Questions = new Dictionary<string, Question>
    {
        ["same_dog"] = Question.Noul("Is the dog in `photo` the same dog as in this picture: 77b0d4a1-poster?"),
        ["has_collar"] = Question.Noul("Is the dog in `photo` wearing a collar?"),
        ["coat"] = Question.Choice(
            "What is the coat of the dog in `photo` like?",
            criteria: new Dictionary<string, object?>
            {
                ["short"] = "Short and smooth.",
                ["long"] = "Long or feathered.",
                ["curly"] = "Curly or wiry."
            }),
        ["collar_box"] = Question.Box("the collar in `photo`")
    },
};

var responseImagesMultipart = await client.EvaluateImagesAsync(
    requestImagesMultipart,
    [
        new MultipartImagePart
        {
            PartName = "dog-found-photo",
            ImageStream = foundDogImageStream,
            FileName = "dog-found.jpg",
            ContentType = "image/jpeg"
        },
        new MultipartImagePart
        {
            PartName = "dog-poster-photo",
            ImageStream = dogPosterImageStream,
            FileName = "dog-poster.png",
            ContentType = "image/png"
        }
    ]);

if (responseImagesMultipart.Answers.TryGetValue("same_dog", out var sameDogAnswer))
{
    Console.WriteLine($"Noul:       {sameDogAnswer.Noul:0.###}");
    Console.WriteLine($"Confidence: {sameDogAnswer.Confidence:0.###}");
}

if (responseImagesMultipart.Answers.TryGetValue("coat", out var coatAnswer))
{
    Console.WriteLine($"Choice:     {coatAnswer.Choice}");
    Console.WriteLine($"Confidence: {coatAnswer.Confidence:0.###}");
}

if (responseImagesMultipart.Answers.TryGetValue("collar_box", out var collarBoxAnswer) && collarBoxAnswer.Box is not null)
{
    Console.WriteLine($"Top:        {collarBoxAnswer.Box.Top:0.###}");
    Console.WriteLine($"Bottom:     {collarBoxAnswer.Box.Bottom:0.###}");
    Console.WriteLine($"Left:       {collarBoxAnswer.Box.Left:0.###}");
    Console.WriteLine($"Right:      {collarBoxAnswer.Box.Right:0.###}");
    Console.WriteLine($"Confidence: {collarBoxAnswer.Confidence:0.###}");
}
```

---

### Sponsors

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=StefH) and [Dapper Plus](https://dapper-plus.net/?utm_source=StefH) are major sponsors and proud to contribute to the development of ****TypeSafe-AI.Sdk****.

[![Entity Framework Extensions](https://raw.githubusercontent.com/StefH/resources/main/sponsor/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=StefH)

[![Dapper Plus](https://raw.githubusercontent.com/StefH/resources/main/sponsor/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=StefH)
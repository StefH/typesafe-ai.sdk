using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TypeSafeAI.Sdk.Api;
using TypeSafeAI.Sdk.Contracts;
using TypeSafeAI.Sdk.DependencyInjection;
using TypeSafeAI.Sdk.Example;

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
    options.EnableImagesDangerZone = true;
});

using var host = builder.Build();

var client = host.Services.GetRequiredService<ITypeSafeClient>();

var requestNoul = new EvaluateRequest
{
    State = "Help! My payouts have been failing for 3 days.",
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

Console.WriteLine(new string('-', 80));

var requestChoice = new EvaluateRequest
{
    State = "My running shoes arrived in the wrong size. Can I swap them for a size 10?",
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

Console.WriteLine(new string('-', 80));

var requestScore = new EvaluateRequest
{
    State = "The export button crashes the settings page in Safari. It works in Chrome, but a few of our customers only use Safari.",
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

Console.WriteLine($"Model:          {responseScore.Model}");
Console.WriteLine($"Usage:          InputTokens={responseScore.Usage.InputTokens}, OutputTokens={responseScore.Usage.OutputTokens}");

Console.WriteLine(new string('-', 80));

var dinoChoice = new EvaluateRequest
{
    State = new
    {
        speed = 4.42f,
        speedMode = "slow",
        dinosaurMotion = "running",
        obstacle = new
        {
            kind = "large_cactus",
            group = "double",
            flightPath = "ground_hazard"
        }
    },
    Questions = new Dictionary<string, Question>
    {
        ["maneuver"] = Question.Choice(
            """
            Choose the single safest maneuver for the dinosaur to avoid,
            the target obstacle and continue running.
            The dinosaur motion in the state is only what it was doing when the
            distant obstacle was first observed; do not assume that motion will
            still be active when the obstacle arrives.
            Choose only the maneuver type. Browser code will handle the exact timing.
            """,
            criteria: new Dictionary<string, object?>
            {
                ["jump"] = "Jump over a ground hazard or an airborne obstacle whose path blocks both a running and ducking dinosaur.",
                ["duck"] = "Duck under an airborne obstacle whose path blocks a running dinosaur but leaves safe space for a ducking dinosaur.",
                ["keep_running"] = "Keep running without jumping or ducking when the obstacle safely clears the running dinosaur."
            }
        ),

        ["jump_profile"] = Question.Choice(
            """
            Assume the safest maneuver is to jump.
            Choose the jump trajectory that best clears the target obstacle.
            Browser code will calculate the exact launch time from the game speed.
            """,
            criteria: new Dictionary<string, object?>
            {
                ["short_jump"] = "Use only for one small cactus. Do not use for a large cactus, grouped cacti, or a pterodactyl.",
                ["full_jump"] = "Use maximum safe airtime for every large cactus, grouped cactus, pterodactyl, or uncertain obstacle."
            }
        )
    },
};

var dinoResponseChoice = await client.EvaluateAsync(dinoChoice);

Console.WriteLine($"State:         {dinoChoice.State}");

if (dinoResponseChoice.Answers.TryGetValue("maneuver", out var maneuverChoice))
{
    Console.WriteLine($"Type:          {maneuverChoice.Type}");
    Console.WriteLine($"Choice:        {maneuverChoice.Choice}");
    Console.WriteLine($"Confidence:    {maneuverChoice.Confidence:0.###}");
    Console.WriteLine($"Probabilities: {JsonSerializer.Serialize(maneuverChoice.Probabilities, options)}");
}

if (dinoResponseChoice.Answers.TryGetValue("jump_profile", out var jumpProfileChoice))
{
    Console.WriteLine($"Type:          {jumpProfileChoice.Type}");
    Console.WriteLine($"Choice:        {jumpProfileChoice.Choice}");
    Console.WriteLine($"Confidence:    {jumpProfileChoice.Confidence:0.###}");
    Console.WriteLine($"Probabilities: {JsonSerializer.Serialize(jumpProfileChoice.Probabilities, options)}");
}

Console.WriteLine($"Usage:          InputTokens={dinoResponseChoice.Usage.InputTokens}, OutputTokens={dinoResponseChoice.Usage.OutputTokens}");

Console.WriteLine(new string('-', 80));

var imagePath = Path.Combine(AppContext.BaseDirectory, "mug.jpg");
var imageBytes = await File.ReadAllBytesAsync(imagePath);
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
            Format = "jpg",
            Base64 = imageBase64
        }
    },
    Questions = new Dictionary<string, Question>
    {
        ["mug"] = Question.Box("the mug in `photo`")
    }
};

var responseBox = await client.EvaluateAsync(requestBox);

Console.WriteLine($"State:         {JsonSerializer.Serialize(requestBox.State, options)}");

if (responseBox.Answers.TryGetValue("mug", out var answerBox))
{
    Console.WriteLine($"Type:          {answerBox.Type}");
    Console.WriteLine($"Confidence:    {answerBox.Confidence:0.###}");

    if (answerBox.Box is not null)
    {
        Console.WriteLine($"Box.Top:       {answerBox.Box.Top:0.###}");
        Console.WriteLine($"Box.Bottom:    {answerBox.Box.Bottom:0.###}");
        Console.WriteLine($"Box.Left:      {answerBox.Box.Left:0.###}");
        Console.WriteLine($"Box.Right:     {answerBox.Box.Right:0.###}");
    }
}

Console.WriteLine($"Model:         {responseBox.Model}");
Console.WriteLine($"Usage:         InputTokens={responseBox.Usage.InputTokens}, OutputTokens={responseBox.Usage.OutputTokens}");

Console.WriteLine(new string('-', 80));

await using var imageStream = File.OpenRead(imagePath);

requestBox = new EvaluateRequest
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
        ["mug"] = Question.Box("the mug in `photo`")
    }
};

responseBox = await client.EvaluateBoxAsync(requestBox, "mug-photo-part", imageStream, "mug.jpg", "image/jpeg");

Console.WriteLine($"State:         {JsonSerializer.Serialize(requestBox.State, options)}");

if (responseBox.Answers.TryGetValue("mug", out answerBox))
{
    Console.WriteLine($"Type:          {answerBox.Type}");
    Console.WriteLine($"Confidence:    {answerBox.Confidence:0.###}");

    if (answerBox.Box is not null)
    {
        Console.WriteLine($"Box.Top:       {answerBox.Box.Top:0.###}");
        Console.WriteLine($"Box.Bottom:    {answerBox.Box.Bottom:0.###}");
        Console.WriteLine($"Box.Left:      {answerBox.Box.Left:0.###}");
        Console.WriteLine($"Box.Right:     {answerBox.Box.Right:0.###}");

        var boxedMugImagePath = Path.Combine(AppContext.BaseDirectory, "mug-box.jpg");
        await ImageUtils.DrawRedBoxAsync(imagePath, boxedMugImagePath, answerBox.Box);

        Console.WriteLine($"Saved image:   {boxedMugImagePath}");
    }
}

Console.WriteLine($"Model:         {responseBox.Model}");
Console.WriteLine($"Usage:         InputTokens={responseBox.Usage.InputTokens}, OutputTokens={responseBox.Usage.OutputTokens}");

Console.WriteLine(new string('-', 80));

var foundDogImagePath = Path.Combine(AppContext.BaseDirectory, "dog-found.jpg");
var dogPosterImagePath = Path.Combine(AppContext.BaseDirectory, "dog-poster.png");

await using var foundDogImageStream = File.OpenRead(foundDogImagePath);
await using var dogPosterImageStream = File.OpenRead(dogPosterImagePath);

var requestImagesMultipart = new EvaluateRequest
{
    Model = "jev-preview",
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
        ["collar_box"] = Question.Box("the collar in `photo`"),
        ["eyes_box"] = Question.Box("the dog's eyes in `photo`")
    }
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

Console.WriteLine($"State:         {JsonSerializer.Serialize(requestImagesMultipart.State, options)}");

if (responseImagesMultipart.Answers.TryGetValue("same_dog", out var sameDogAnswer))
{
    Console.WriteLine($"Key:           same_dog");
    Console.WriteLine($"Type:          {sameDogAnswer.Type}");
    Console.WriteLine($"Noul:          {sameDogAnswer.Noul:0.###}");
}

Console.WriteLine();

if (responseImagesMultipart.Answers.TryGetValue("has_collar", out var hasCollarAnswer))
{
    Console.WriteLine($"Key:           has_collar");
    Console.WriteLine($"Type:          {hasCollarAnswer.Type}");
    Console.WriteLine($"Noul:          {hasCollarAnswer.Noul:0.###}");
}

Console.WriteLine();

if (responseImagesMultipart.Answers.TryGetValue("coat", out var coatAnswer))
{
    Console.WriteLine($"Key:           coat");
    Console.WriteLine($"Type:          {coatAnswer.Type}");
    Console.WriteLine($"Choice:        {coatAnswer.Choice}");
    Console.WriteLine($"Confidence:    {coatAnswer.Confidence:0.###}");
    Console.WriteLine($"Probabilities: {JsonSerializer.Serialize(coatAnswer.Probabilities, options)}");
}

Console.WriteLine();

if (responseImagesMultipart.Answers.TryGetValue("collar_box", out var collarBoxAnswer) && collarBoxAnswer.Box is not null)
{
    Console.WriteLine($"Key:        collar_box");
    Console.WriteLine($"Top:        {collarBoxAnswer.Box.Top:0.###}");
    Console.WriteLine($"Bottom:     {collarBoxAnswer.Box.Bottom:0.###}");
    Console.WriteLine($"Left:       {collarBoxAnswer.Box.Left:0.###}");
    Console.WriteLine($"Right:      {collarBoxAnswer.Box.Right:0.###}");
    Console.WriteLine($"Confidence: {collarBoxAnswer.Confidence:0.###}");

    var boxedCollarImagePath = Path.Combine(AppContext.BaseDirectory, "dog-found-collar-box.jpg");
    await ImageUtils.DrawRedBoxAsync(foundDogImagePath, boxedCollarImagePath, collarBoxAnswer.Box);

    Console.WriteLine($"Saved image: {boxedCollarImagePath}");
}

Console.WriteLine();

if (responseImagesMultipart.Answers.TryGetValue("eyes_box", out var eyesBoxAnswer) && eyesBoxAnswer.Box is not null)
{
    Console.WriteLine($"Key:        eyes_box");
    Console.WriteLine($"Top:        {eyesBoxAnswer.Box.Top:0.###}");
    Console.WriteLine($"Bottom:     {eyesBoxAnswer.Box.Bottom:0.###}");
    Console.WriteLine($"Left:       {eyesBoxAnswer.Box.Left:0.###}");
    Console.WriteLine($"Right:      {eyesBoxAnswer.Box.Right:0.###}");
    Console.WriteLine($"Confidence: {eyesBoxAnswer.Confidence:0.###}");

    var boxedEyesImagePath = Path.Combine(AppContext.BaseDirectory, "dog-found-eyes-box.jpg");
    await ImageUtils.DrawRedBoxAsync(foundDogImagePath, boxedEyesImagePath, eyesBoxAnswer.Box);

    Console.WriteLine($"Saved image: {boxedEyesImagePath}");
}

Console.WriteLine($"Model:         {responseImagesMultipart.Model}");
Console.WriteLine($"Usage:         InputTokens={responseImagesMultipart.Usage.InputTokens}, OutputTokens={responseImagesMultipart.Usage.OutputTokens}");

Console.WriteLine(new string('-', 80));

var tester = new PromptInjectionTester(client);
await tester.RunsTestsAsync();

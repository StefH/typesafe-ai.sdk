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

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTypeSafeSdk(options =>
{
    options.ApiKey = apiKey;
});

using var host = builder.Build();

var client = host.Services.GetRequiredService<ITypeSafeClient>();
```

### Create a Noulrequest
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

if (responseNoul.Answers.TryGetValue("is_urgent", out var answerNoul))
{
    Console.WriteLine($"Type:       {answerNoul.Type}");
    Console.WriteLine($"Noul:       {answerNoul.Noul:0.###}");
    Console.WriteLine($"Confidence: {answerNoul.Confidence:0.###}");
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

if (responseChoice.Answers.TryGetValue("instructions", out var answerChoice))
{
    Console.WriteLine($"Type:       {answerChoice.Type}");
    Console.WriteLine($"Choice:     {answerChoice.Choice}");
    Console.WriteLine($"Confidence: {answerChoice.Confidence:0.###}");
}

Console.WriteLine($"Model: {responseChoice.Model}");
Console.WriteLine($"Usage: InputTokens={responseChoice.Usage.InputTokens}, OutputTokens={responseChoice.Usage.OutputTokens}");
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

if (responseScore.Answers.TryGetValue("bug_severity", out var answerScore))
{
    Console.WriteLine($"Type:       {answerScore.Type}");
    Console.WriteLine($"Score:      {answerScore.Score}");
    Console.WriteLine($"Confidence: {answerScore.Confidence:0.###}");
}

Console.WriteLine($"Model: {responseScore.Model}");
Console.WriteLine($"Usage: InputTokens={responseScore.Usage.InputTokens}, OutputTokens={responseScore.Usage.OutputTokens}");
```

---

### Sponsors

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=StefH) and [Dapper Plus](https://dapper-plus.net/?utm_source=StefH) are major sponsors and proud to contribute to the development of ****TypeSafe-AI.Sdk****.

[![Entity Framework Extensions](https://raw.githubusercontent.com/StefH/resources/main/sponsor/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=StefH)

[![Dapper Plus](https://raw.githubusercontent.com/StefH/resources/main/sponsor/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=StefH)
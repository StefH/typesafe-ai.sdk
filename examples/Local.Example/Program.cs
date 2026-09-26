using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TypeSafeAI.Sdk.Api;
using TypeSafeAI.Sdk.Contracts;
using TypeSafeAI.Sdk.DependencyInjection;

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
    options.Endpoint = new Uri("http://localhost:1234");
    options.ApiKey = apiKey;
});

using var host = builder.Build();

var client = host.Services.GetRequiredService<ITypeSafeClient>();

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

Console.WriteLine(new string('-', 80));

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

Console.WriteLine(new string('-', 80));

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

Console.WriteLine($"Model:          {responseScore.Model}");
Console.WriteLine($"Usage:          InputTokens={responseScore.Usage.InputTokens}, OutputTokens={responseScore.Usage.OutputTokens}");
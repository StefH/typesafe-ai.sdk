using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TypeSafeAI.Sdk.Contracts;
using TypeSafeAI.Sdk.DependencyInjection;
using TypeSafeAI.Sdk.Services;

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

var request = new EvaluateRequest
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

var response = await client.EvaluateAsync(request);

if (response.Answers.TryGetValue("is_urgent", out var answer))
{
    Console.WriteLine($"Urgency score: {answer.Noul:0.###}");
}

Console.WriteLine($"Model: {response.Model}");
Console.WriteLine($"Usage: in={response.Usage.InputTokens}, out={response.Usage.OutputTokens}");

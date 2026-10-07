using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TypeSafeAI.Sdk.Api;
using TypeSafeAI.Sdk.Contracts;
using TypeSafeAI.Sdk.DependencyInjection;

string apiKey = string.Empty;
string url;
string? model = null;

Console.WriteLine("Choose Runner / AI Model");
Console.WriteLine("1. Unsloth");
Console.WriteLine("2. Ollama : Nimble");
Console.WriteLine("3. Llama : Lev");
Console.WriteLine("0. Quit");

var option = Console.Read();
if (option == '0')
{
    Environment.Exit(0);
    return;
}

if (option == '1')
{
    url = Environment.GetEnvironmentVariable("UNSLOTH_URL")!;
    apiKey = Environment.GetEnvironmentVariable("UNSLOTH_API_KEY")!;
    model = "jev-latest";
}
else if (option == '2')
{
    url = "http://localhost:11434";
    model = "nimble";
}
else if (option >= '3')
{
    url = "http://localhost:8080";
}
else
{
    Console.WriteLine("Invalid option.");
    return;
}

var options = new JsonSerializerOptions { WriteIndented = true };

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTypeSafeSdk(options =>
{
    options.BaseAddress = new Uri(url);
    options.ApiKey = apiKey;
});

using var host = builder.Build();

var client = host.Services.GetRequiredService<ITypeSafeClient>();

var requestNoul = new EvaluateRequest
{
    Model = model,
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

Console.WriteLine($"Usage: InputTokens={responseNoul.Usage.InputTokens}");

Console.WriteLine(new string('-', 80));

var requestChoice = new EvaluateRequest
{
    Model = model,
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

Console.WriteLine($"Usage:          InputTokens={responseChoice.Usage.InputTokens}");

Console.WriteLine(new string('-', 80));

var requestScore = new EvaluateRequest
{
    Model = model,
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

Console.WriteLine($"Usage:          InputTokens={responseScore.Usage.InputTokens}");

Console.WriteLine(new string('-', 80));

var dinoChoice = new EvaluateRequest
{
    Model = model,
    State = new
    {
        speed = 4.42,
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
            Choose the single safest maneuver for the dinosaur to avoid, the target obstacle and continue running.
            The dinosaur motion in the state is only what it was doing when the distant obstacle was first observed;
            do not assume that motion will still be active when the obstacle arrives.
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

Console.WriteLine($"Usage:          InputTokens={dinoResponseChoice.Usage.InputTokens}");

Console.WriteLine(new string('-', 80));
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

var azureOpenAiUrl = Environment.GetEnvironmentVariable("AZURE_OPENAI_URL2") ?? string.Empty;
var azureOpenAiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY2") ?? string.Empty;

if (string.IsNullOrWhiteSpace(azureOpenAiUrl) || string.IsNullOrWhiteSpace(azureOpenAiKey))
{
    Console.WriteLine("Please set AZURE_OPENAI_URL2 and AZURE_OPENAI_KEY2 before running the example.");
    return;
}

var openAiClient = new AzureOpenAIClient(new Uri(azureOpenAiUrl), new AzureKeyCredential(azureOpenAiKey));
var chatClient = openAiClient.GetChatClient("gpt-5-mini");

// Example 1: Noul (Yes/No) evaluation
Console.WriteLine("=== Example 1: Noul (Yes/No) Evaluation ===");

var stateNoul = "Help! My payouts have been failing for 3 days.";
var systemPromptNoul = """
You are an evaluator that answers binary questions about statements.
Your response should be a JSON object with this structure:
{
  "type": "noul",
  "noul": 0.0 to 1.0 (confidence score),
  "confidence": 0.0 to 1.0
}
""";

var messageNoul = $"""
State: {stateNoul}

Question: Does this convey urgency?
Yes criteria: Explicitly time-sensitive
No criteria: No urgency expressed

Respond with only valid JSON.
""";

var messagesNoul = new List<ChatMessage>
{
    new SystemChatMessage(systemPromptNoul),
    new UserChatMessage(messageNoul)
};

var responseNoul = await chatClient.CompleteChatAsync(messagesNoul);

Console.WriteLine($"State: {stateNoul}");
Console.WriteLine($"Response: {responseNoul.Value.Content[0].Text}");
Console.WriteLine();

// Example 2: Choice evaluation
Console.WriteLine("=== Example 2: Choice Evaluation ===");

var stateChoice = "My running shoes arrived in the wrong size. Can I swap them for a size 10?";
var systemPromptChoice = """
You are an evaluator that categorizes statements into predefined choices.
Your response should be a JSON object with this structure:
{
  "type": "choice",
  "choice": "one of the provided options",
  "confidence": 0.0 to 1.0,
  "probabilities": {
    "option1": 0.0 to 1.0,
    "option2": 0.0 to 1.0,
    ...
  }
}
""";

var messageChoice = $"""
State: {stateChoice}

Question: Which team should handle this?
Options:
- returns: Exchanges, wrong or damaged items
- shipping: Delivery status, delays, lost packages
- billing: Charges, invoices, payment problems

Respond with only valid JSON.
""";

var messagesChoice = new List<ChatMessage>
{
    new SystemChatMessage(systemPromptChoice),
    new UserChatMessage(messageChoice)
};

var responseChoice = await chatClient.CompleteChatAsync(messagesChoice);

Console.WriteLine($"State: {stateChoice}");
Console.WriteLine($"Response: {responseChoice.Value.Content[0].Text}");
Console.WriteLine();

Console.WriteLine(new string('-', 80));
Console.WriteLine();

// Example 3: Score evaluation
Console.WriteLine("=== Example 3: Score Evaluation ===");

var stateScore = "The export button crashes the settings page in Safari. It works in Chrome, but a few of our customers only use Safari.";
var systemPromptScore = """
You are an evaluator that scores statements on a scale based on predefined criteria.
Your response should be a JSON object with this structure:
{
  "type": "score",
  "score": 0, 1, or 2 (based on criteria index),
  "confidence": 0.0 to 1.0,
  "legend": {
    "0": "Cosmetic; no impact to functionality",
    "1": "Broken or degraded feature, but workaround exists",
    "2": "Blocking issue; no workaround exists"
  },
  "probabilities": {
    "0": 0.0 to 1.0,
    "1": 0.0 to 1.0,
    "2": 0.0 to 1.0
  }
}
""";

var messageScore = $"""
State: {stateScore}

Question: How severe is the reported issue?
Severity levels:
0. Cosmetic; no impact to functionality
1. Broken or degraded feature, but workaround exists
2. Blocking issue; no workaround exists

Respond with only valid JSON.
""";

var messagesScore = new List<ChatMessage>
{
    new SystemChatMessage(systemPromptScore),
    new UserChatMessage(messageScore)
};

var responseScore = await chatClient.CompleteChatAsync(messagesScore);

Console.WriteLine($"State: {stateScore}");
Console.WriteLine($"Response: {responseScore.Value.Content[0].Text}");

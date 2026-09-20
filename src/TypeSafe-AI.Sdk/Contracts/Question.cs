using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

public sealed class Question
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("instructions")]
    public required object Instructions { get; init; }

    [JsonPropertyName("criteria")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Criteria { get; init; }

    public static Question Noul(object instructions, object? yesCriteria = null, object? noCriteria = null)
    {
        var criteria = (yesCriteria, noCriteria) switch
        {
            (null, null) => null,
            _ => new Dictionary<string, object?>
            {
                ["true"] = yesCriteria,
                ["false"] = noCriteria
            }
        };

        return new Question
        {
            Type = "noul",
            Instructions = instructions,
            Criteria = criteria
        };
    }

    public static Question Choice(object instructions, IReadOnlyDictionary<string, object?> criteria) => new()
    {
        Type = "choice",
        Instructions = instructions,
        Criteria = criteria
    };

    public static Question Score(object instructions, IReadOnlyList<object> criteria) => new()
    {
        Type = "score",
        Instructions = instructions,
        Criteria = criteria
    };
}
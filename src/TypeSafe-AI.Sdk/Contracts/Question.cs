using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

/// <summary>
/// A <c>Question</c> is one of three types, set by its <c>type</c> field. All three share <c>type</c> and <c>instructions</c>; each adds its own <c>criteria</c>.
/// </summary>
public sealed class Question
{
    /// <summary>
    /// A <c>Question</c> is one of three types, set by its <c>type</c> field.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// The <c>instructions</c> property can be a string, an object, or an array.
    /// </summary>
    [JsonPropertyName("instructions")]
    public required object Instructions { get; init; }

    /// <summary>
    /// All three share <c>type</c> and <c>instructions</c>; each adds its own <c>criteria</c>.
    /// </summary>
    [JsonPropertyName("criteria")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Criteria { get; init; }

    /// <summary>
    /// Creates a new <c>Question</c> of type "noul" (yes/no) with the given instructions and optional criteria for yes and no answers.
    /// </summary>
    /// <param name="instructions">The instructions for the question.</param>
    /// <param name="yesCriteria">The criteria for a "yes" answer.</param>
    /// <param name="noCriteria">The criteria for a "no" answer.</param>
    /// <returns>A new <c>Question</c> of type "noul".</returns>
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

    /// <summary>
    /// Creates a new <c>Question</c> of type "choice" with the given instructions and criteria for each choice.
    /// </summary>
    /// <param name="instructions">The instructions for the question.</param>
    /// <param name="criteria">The criteria for each choice.</param>
    /// <returns>A new <c>Question</c> of type "choice".</returns>
    public static Question Choice(object instructions, IReadOnlyDictionary<string, object?> criteria) => new()
    {
        Type = "choice",
        Instructions = instructions,
        Criteria = criteria
    };

    /// <summary>
    /// Creates a new <c>Question</c> of type "score" with the given instructions and criteria for scoring.
    /// </summary>
    /// <param name="instructions">The instructions for the question.</param>
    /// <param name="criteria">The criteria for scoring.</param>
    /// <returns>A new <c>Question</c> of type "score".</returns>
    public static Question Score(object instructions, IReadOnlyList<object> criteria) => new()
    {
        Type = "score",
        Instructions = instructions,
        Criteria = criteria
    };
}
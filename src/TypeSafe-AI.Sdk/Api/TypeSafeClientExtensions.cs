using TypeSafeAI.Sdk.Contracts;

namespace TypeSafeAI.Sdk.Api;

/// <summary>
/// Extension methods for <c>ITypeSafeClient</c>.
/// </summary>
public static class TypeSafeClientExtensions
{
    /// <summary>
    /// Evaluate a <c>state</c> against a typed <c>Question</c> and get back a structured <c>Answer</c>.
    /// </summary>
    /// <param name="state">The top-level shape of the request.</param>
    /// <param name="key">The key for the question.</param>
    /// <param name="question">The question to evaluate.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The answer for the question.</returns>
    public static async Task<EvaluateResponse> EvaluateQuestionAsync(this ITypeSafeClient client, object state, string key, Question question, CancellationToken cancellationToken = default)
    {
        var request = new EvaluateRequest
        {
            State = state,
            Questions = new Dictionary<string, Question> { { key, question } }
        };
        return await client.EvaluateAsync(request, cancellationToken);
    }
}
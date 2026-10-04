using System.ComponentModel.DataAnnotations;

namespace TypeSafeAI.Sdk;

public class TypeSafeOptions
{
    /// <summary>
    /// The base address of the TypeSafe API.
    /// </summary>
    [Url]
    public required Uri BaseAddress { get; set; } = new("https://api.typesafe.ai");

    /// <summary>
    /// The optional API key to use for authentication. If not provided, the client does not use any authentication.
    /// </summary>
    public string? ApiKey { get; set; }
}
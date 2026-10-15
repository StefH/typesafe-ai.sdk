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

    /// <summary>
    /// To use images or Box questions, send the <c>TypeSafe-Danger-Zone-Images: true</c> header with each request that contains an image or a Box question.
    /// </summary>
    public bool EnableImagesDangerZone { get; set; }
}

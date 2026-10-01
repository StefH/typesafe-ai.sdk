using System.ComponentModel.DataAnnotations;

namespace TypeSafeAI.Sdk;

public class TypeSafeOptions
{
    [Url]
    public required Uri BaseAddress { get; set; } = new("https://api.typesafe.ai");

    [Required]
    public required string ApiKey { get; set; } 
}
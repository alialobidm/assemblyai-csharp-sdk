using System.Text.Json.Serialization;

#nullable enable

namespace AssemblyAI.Lemur;

public record LemurUsage
{
    /// <summary>
    /// The number of input tokens used by the model
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public required int InputTokens { get; set; }

    /// <summary>
    /// The number of output tokens generated by the model
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public required int OutputTokens { get; set; }
}
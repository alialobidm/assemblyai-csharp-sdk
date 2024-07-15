namespace AssemblyAI;

public record GetSubtitlesParams
{
    /// <summary>
    /// The maximum number of characters per caption
    /// </summary>
    public int? CharsPerCaption { get; init; }
}

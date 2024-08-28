using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using AssemblyAI.Core;
using AssemblyAI.Files;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace AssemblyAI.Transcripts;

/// <summary>
/// The client to interact with the AssemblyAI Transcripts API.
/// </summary>
public class ExtendedTranscriptsClient : TranscriptsClient
{
    private readonly RawClient _client;
    private readonly AssemblyAIClient _assemblyAIClient;

    internal ExtendedTranscriptsClient(RawClient client, AssemblyAIClient assemblyAIClient) : base(client)
    {
        _client = client;
        _assemblyAIClient = assemblyAIClient;
    }
    
    /// <summary>
    /// Create a transcript from a local file.
    /// </summary>
    /// <param name="audioFile">The audio file to transcribe</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public Task<Transcript> SubmitAsync(FileInfo audioFile) => SubmitAsync(audioFile, new TranscriptOptionalParams());
    
    /// <summary>
    /// Create a transcript from a local file.
    /// </summary>
    /// <param name="audioFile">The audio file to transcribe</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public async Task<Transcript> SubmitAsync(FileInfo audioFile, TranscriptOptionalParams transcriptParams)
    {
        var uploadedFile = await _assemblyAIClient.Files.UploadAsync(audioFile).ConfigureAwait(false);
        return await SubmitAsync(uploadedFile, transcriptParams).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a transcript from a file stream.
    /// </summary>
    /// <param name="audioFileStream">The audio file stream to transcribe</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public Task<Transcript> SubmitAsync(Stream audioFileStream) =>
        SubmitAsync(audioFileStream, new TranscriptOptionalParams());

    /// <summary>
    /// Create a transcript from a file stream.
    /// </summary>
    /// <param name="audioFileStream">The audio file stream to transcribe</param>
    /// <param name="disposeStream">Dispose the stream as soon as possible</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public Task<Transcript> SubmitAsync(Stream audioFileStream, bool disposeStream) =>
        SubmitAsync(audioFileStream, disposeStream, new TranscriptOptionalParams());

    /// <summary>
    /// Create a transcript from a file stream.
    /// </summary>
    /// <param name="audioFileStream">The audio file stream to transcribe</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public Task<Transcript> SubmitAsync(Stream audioFileStream, TranscriptOptionalParams transcriptParams)
        => SubmitAsync(audioFileStream, false, transcriptParams);

    /// <summary>
    /// Create a transcript from a file stream.
    /// </summary>
    /// <param name="audioFileStream">The audio file stream to transcribe</param>
    /// <param name="disposeStream">Dispose the stream as soon as possible</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public async Task<Transcript> SubmitAsync(
        Stream audioFileStream,
        bool disposeStream,
        TranscriptOptionalParams transcriptParams
    )
    {
        var fileUpload = await _assemblyAIClient.Files.UploadAsync(audioFileStream, disposeStream)
            .ConfigureAwait(false);
        return await SubmitAsync(fileUpload, transcriptParams).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Create a transcript from an audio file URI.
    /// </summary>
    /// <param name="audioFileUrl">The URI to the audio file to transcribe</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public Task<Transcript> SubmitAsync(Uri audioFileUrl) => SubmitAsync(audioFileUrl, new TranscriptOptionalParams());

    /// <summary>
    /// Create a transcript from an audio file URI.
    /// </summary>
    /// <param name="audioFileUrl">The URI to the audio file to transcribe</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public async Task<Transcript> SubmitAsync(Uri audioFileUrl, TranscriptOptionalParams transcriptParams)
    {
        return await SubmitAsync(CreateParams(audioFileUrl, transcriptParams)).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a transcript from a file uploaded to AssemblyAI.
    /// </summary>
    /// <param name="file">The file uploaded to AssemblyAI</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public Task<Transcript> SubmitAsync(UploadedFile file) => SubmitAsync(file, new TranscriptOptionalParams());

    /// <summary>
    /// Create a transcript from a file uploaded to AssemblyAI.
    /// </summary>
    /// <param name="file">The file uploaded to AssemblyAI</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>Returns a task that resolves to a queued transcript</returns>
    public async Task<Transcript> SubmitAsync(UploadedFile file, TranscriptOptionalParams transcriptParams)
    {
        return await SubmitAsync(CreateParams(file.UploadUrl, transcriptParams)).ConfigureAwait(false);
    }

    /// <summary>
    /// Transcribe a local file
    /// </summary>
    /// <param name="audioFile">The local file to transcribe</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public Task<Transcript> TranscribeAsync(FileInfo audioFile) =>
        TranscribeAsync(audioFile, new TranscriptOptionalParams());

    /// <summary>
    /// Transcribe a local file
    /// </summary>
    /// <param name="audioFile">The local file to transcribe</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public async Task<Transcript> TranscribeAsync(FileInfo audioFile, TranscriptOptionalParams transcriptParams)
    {
        using var audioFileStream = audioFile.OpenRead();
        return await TranscribeAsync(audioFileStream, transcriptParams).ConfigureAwait(false);
    }

    /// <summary>
    /// Transcribe a file stream.
    /// </summary>
    /// <param name="audioFileStream">The audio file stream to transcribe</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public Task<Transcript> TranscribeAsync(Stream audioFileStream) =>
        TranscribeAsync(audioFileStream, new TranscriptOptionalParams());

    /// <summary>
    /// Transcribe a file stream.
    /// </summary>
    /// <param name="audioFileStream">The audio file stream to transcribe</param>
    /// <param name="disposeStream">Dispose the stream as soon as possible</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public Task<Transcript> TranscribeAsync(Stream audioFileStream, bool disposeStream) =>
        TranscribeAsync(audioFileStream, disposeStream, new TranscriptOptionalParams());

    /// <summary>
    /// Transcribe a file stream.
    /// </summary>
    /// <param name="audioFileStream">The audio file stream to transcribe</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public async Task<Transcript> TranscribeAsync(Stream audioFileStream, TranscriptOptionalParams transcriptParams)
    {
        var fileUpload = await _assemblyAIClient.Files.UploadAsync(audioFileStream).ConfigureAwait(false);
        return await TranscribeAsync(new Uri(fileUpload.UploadUrl), transcriptParams).ConfigureAwait(false);
    }

    /// <summary>
    /// Transcribe a file stream.
    /// </summary>
    /// <param name="audioFileStream">The audio file stream to transcribe</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <param name="disposeStream">Dispose the stream as soon as possible</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public async Task<Transcript> TranscribeAsync(Stream audioFileStream, bool disposeStream,
        TranscriptOptionalParams transcriptParams)
    {
        var uploadedFile =
            await _assemblyAIClient.Files.UploadAsync(audioFileStream, disposeStream).ConfigureAwait(false);
        return await TranscribeAsync(uploadedFile, transcriptParams).ConfigureAwait(false);
    }

    /// <summary>
    /// Transcribe an audio file via its public URI.
    /// </summary>
    /// <param name="audioFileUrl">The URI to the audio file to transcribe</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public Task<Transcript> TranscribeAsync(Uri audioFileUrl) =>
        TranscribeAsync(audioFileUrl, new TranscriptOptionalParams());

    /// <summary>
    /// Transcribe an audio file via its public URI.
    /// </summary>
    /// <param name="audioFileUrl">The URI to the audio file to transcribe</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public Task<Transcript> TranscribeAsync(Uri audioFileUrl, TranscriptOptionalParams transcriptParams)
        => TranscribeAsync(CreateParams(audioFileUrl, transcriptParams));

    /// <summary>
    /// Transcribe a file uploaded to AssemblyAI.
    /// </summary>
    /// <param name="file">The file uploaded to AssemblyAI to transcribe</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public Task<Transcript> TranscribeAsync(UploadedFile file) =>
        TranscribeAsync(file, new TranscriptOptionalParams());


    /// <summary>
    /// Transcribe a file uploaded to AssemblyAI.
    /// </summary>
    /// <param name="file">The file uploaded to AssemblyAI to transcribe</param>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public Task<Transcript> TranscribeAsync(UploadedFile file, TranscriptOptionalParams transcriptParams)
        => TranscribeAsync(CreateParams(file.UploadUrl, transcriptParams));

    /// <summary>
    /// Transcribe an audio file via its public URI.
    /// </summary>
    /// <param name="transcriptParams">The transcript parameters</param>
    /// <returns>A task that resolves to a transcript with status "completed" or "error".</returns>
    public async Task<Transcript> TranscribeAsync(TranscriptParams transcriptParams)
    {
        var transcript = await SubmitAsync(transcriptParams).ConfigureAwait(false);
        transcript = await WaitUntilReady(transcript.Id).ConfigureAwait(false);
        return transcript;
    }

    /// <summary>
    /// Wait until the transcript status is either "completed" or "error".
    /// </summary>
    /// <param name="id">The transcript ID</param>
    /// <param name="pollingInterval">How frequently the transcript is polled. Defaults to 3s.</param>
    /// <param name="pollingTimeout">How long to wait until the timeout exception thrown. Defaults to infinite.</param>
    /// <returns>The transcript with status "completed" or "error"</returns>
    public async Task<Transcript> WaitUntilReady(
        string id,
        TimeSpan? pollingInterval = null,
        TimeSpan? pollingTimeout = null
    )
    {
        var ct = pollingTimeout == null
            ? CancellationToken.None
            : new CancellationTokenSource(pollingTimeout.Value).Token;

        var transcript = await GetAsync(id).ConfigureAwait(false);
        while (transcript.Status != TranscriptStatus.Completed && transcript.Status != TranscriptStatus.Error)
        {
            if (ct.IsCancellationRequested)
            {
                throw new TimeoutException("The transcript did not complete within the given timeout.");
            }

            try
            {
                await Task.Delay(pollingInterval ?? TimeSpan.FromSeconds(3), ct).ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                throw new TimeoutException("The transcript did not complete within the given timeout.", e);
            }

            transcript = await GetAsync(transcript.Id).ConfigureAwait(false);
        }

        return transcript;
    }

    /// <summary>
    /// Create transcript parameters from an audio file URL and optional parameters.
    /// </summary>
    /// <param name="audioFileUrl">The audio file URL to transcribe</param>
    /// <param name="optionalTranscriptParams">The optional transcript parameters</param>
    /// <returns>The transcript parameters</returns>
    private static TranscriptParams CreateParams(Uri audioFileUrl, TranscriptOptionalParams optionalTranscriptParams)
        => CreateParams(audioFileUrl.ToString(), optionalTranscriptParams);

    /// <inheritdoc cref="CreateParams(Uri,TranscriptOptionalParams)"/>
    private static TranscriptParams CreateParams(string audioFileUrl, TranscriptOptionalParams optionalTranscriptParams)
    {
        var json = JsonUtils.Serialize(optionalTranscriptParams);
        var jsonObject = JsonUtils.Deserialize<JsonObject>(json);
        jsonObject["audio_url"] = audioFileUrl;
        var transcriptParams = jsonObject.Deserialize<TranscriptParams>()!;
        return transcriptParams;
    }

    /// <summary>
    /// Retrieve a list of transcripts you created.
    /// Transcripts are sorted from newest to oldest. The previous URL always points to a page with older transcripts.
    /// </summary>
    /// <returns>A list of transcripts you created</returns>
    public Task<TranscriptList> ListAsync() => ListAsync(new ListTranscriptParams());

    /// <inheritdoc cref="ListAsync()"/>
    /// <param name="listUrl">The next or previous page URL to query the transcript list.</param>
    public async Task<TranscriptList> ListAsync(string listUrl)
    {
        if (string.IsNullOrEmpty(listUrl))
            throw new ArgumentNullException(nameof(listUrl), "listUrl parameter is null or empty.");

        // this would be easier to just call the given URL,
        // but the raw client doesn't let us make requests to full URL
        // so, we'll parse the querystring and pass it to `ListAsync`.

        var queryString = listUrl.Substring(listUrl.IndexOf('?') + 1)
            .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Split('='))
            .Where(k => k.Length == 2)
            .ToLookup(a => a[0], a => Uri.UnescapeDataString(a[1])
                , StringComparer.OrdinalIgnoreCase);
        var listTranscriptParams = new ListTranscriptParams();
        if (queryString.Contains("limit"))
        {
            listTranscriptParams.Limit = int.Parse(queryString["limit"].First());
        }

        if (queryString.Contains("status"))
        {
            listTranscriptParams.Status =
                (TranscriptStatus)Enum.Parse(typeof(TranscriptStatus), queryString["limit"].First());
        }

        if (queryString.Contains("created_on"))
        {
            listTranscriptParams.CreatedOn = queryString["created_on"].First();
        }

        if (queryString.Contains("before_id"))
        {
            listTranscriptParams.BeforeId = queryString["before_id"].First();
        }

        if (queryString.Contains("after_id"))
        {
            listTranscriptParams.AfterId = queryString["after_id"].First();
        }

        if (queryString.Contains("throttled_only"))
        {
            listTranscriptParams.ThrottledOnly = bool.Parse(queryString["throttled_only"].First());
        }

        return await ListAsync(listTranscriptParams).ConfigureAwait(false);
    }

    /// <summary>
    /// Export your transcript in SRT or VTT format to use with a video player for subtitles and closed captions.
    /// </summary>
    public Task<string> GetSubtitlesAsync(
        string transcriptId,
        SubtitleFormat subtitleFormat
    )
        => GetSubtitlesAsync(transcriptId, subtitleFormat, new GetSubtitlesParams());

    /// <summary>
    /// Export your transcript in SRT or VTT format to use with a video player for subtitles and closed captions.
    /// </summary>
    public Task<string> GetSubtitlesAsync(
        string transcriptId,
        SubtitleFormat subtitleFormat,
        int charsPerCaption
    )
        => GetSubtitlesAsync(transcriptId, subtitleFormat, new GetSubtitlesParams
        {
            CharsPerCaption = charsPerCaption
        });

    /// <summary>
    /// Retrieve the redacted audio file.
    /// </summary>
    public async Task<Stream> GetRedactedAudioFileAsync(
        string transcriptId,
        RequestOptions? options = null
    )
    {
        var redactedAudioFileInfo = await GetRedactedAudioAsync(transcriptId, options).ConfigureAwait(false);
        var httpClient = options?.HttpClient ?? _client.Options.HttpClient ?? new HttpClient();
        return await httpClient.GetStreamAsync(redactedAudioFileInfo.RedactedAudioUrl).ConfigureAwait(false);
    }

    /// <summary>
    /// Search through the transcript for keywords. You can search for individual words, numbers, or phrases containing up to five words or numbers.
    /// </summary>
    public Task<WordSearchResponse> WordSearchAsync(
        string transcriptId,
        string[] words
    ) => WordSearchAsync(transcriptId, new WordSearchParams
    {
        Words = words
    });
}
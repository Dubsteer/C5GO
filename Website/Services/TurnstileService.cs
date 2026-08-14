using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Website.Configuration;

namespace Website.Services;

public sealed class TurnstileService
{
    private readonly HttpClient httpClient;
    private readonly TurnstileOptions options;
    private readonly ILogger<TurnstileService> logger;

    public TurnstileService(
        HttpClient httpClient,
        IOptions<TurnstileOptions> options,
        ILogger<TurnstileService> logger)
    {
        this.httpClient = httpClient;
        this.options = options.Value;
        this.logger = logger;
    }

    public string SiteKey => options.SiteKey;

    public async Task<bool> ValidateAsync(
        string? token,
        string? remoteIp,
        CancellationToken cancellationToken = default)
    {
        if (!options.IsConfigured || string.IsNullOrWhiteSpace(token))
            return false;

        var values = new Dictionary<string, string>
        {
            ["secret"] = options.SecretKey,
            ["response"] = token
        };

        if (IPAddress.TryParse(remoteIp, out var address))
            values["remoteip"] = address.ToString();

        try
        {
            using var content = new FormUrlEncodedContent(values);
            using var response = await httpClient.PostAsync(
                "siteverify",
                content,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Turnstile validation returned HTTP status {StatusCode}.",
                    response.StatusCode);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<TurnstileResponse>(
                cancellationToken: cancellationToken);
            return result?.Success == true;
        }
        catch (Exception exception) when (
            exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            logger.LogWarning(exception, "Turnstile validation could not be completed.");
            return false;
        }
    }

    private sealed class TurnstileResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
    }
}

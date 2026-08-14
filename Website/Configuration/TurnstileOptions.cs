namespace Website.Configuration;

public sealed class TurnstileOptions
{
    public const string SectionName = "Turnstile";
    public const string DevelopmentSiteKey = "1x00000000000000000000AA";
    public const string DevelopmentSecretKey = "1x0000000000000000000000000000000AA";

    public string SiteKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(SiteKey) &&
        !string.IsNullOrWhiteSpace(SecretKey);
}

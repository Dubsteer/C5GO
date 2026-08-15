namespace Website.Configuration;

public sealed class ImageStorageOptions
{
    public const string SectionName = "ImageStorage";

    public string RootPath { get; set; } = string.Empty;
}

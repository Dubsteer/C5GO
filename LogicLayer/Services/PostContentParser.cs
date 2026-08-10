using System.Net;
using System.Text.RegularExpressions;

namespace LogicLayer.Services
{
    public sealed record PostContentBlock(string Text, string? VideoId)
    {
        public bool IsVideo => VideoId != null;
    }

    public static partial class PostContentParser
    {
        private static readonly char[] TrailingUrlCharacters = ['.', ',', '!', '?', ')', ']', '}', ';', ':'];

        public static IReadOnlyList<PostContentBlock> Parse(string? content)
        {
            var plainText = ToPlainText(content);
            var blocks = new List<PostContentBlock>();
            var textStart = 0;

            foreach (Match match in UrlPattern().Matches(plainText))
            {
                var url = match.Value.TrimEnd(TrailingUrlCharacters);
                if (!TryGetYouTubeVideoId(url, out var videoId))
                    continue;

                AddText(blocks, plainText[textStart..match.Index]);
                blocks.Add(new PostContentBlock(string.Empty, videoId));

                var trailingCharacters = match.Value[url.Length..];
                textStart = match.Index + match.Length;
                if (trailingCharacters.Length > 0)
                    blocks.Add(new PostContentBlock(trailingCharacters, null));
            }

            AddText(blocks, plainText[textStart..]);

            if (blocks.Count == 0)
                blocks.Add(new PostContentBlock(string.Empty, null));

            return blocks;
        }

        public static string GetPreviewText(string? content)
        {
            var blocks = Parse(content);
            var text = string.Join(" ", blocks.Where(block => !block.IsVideo).Select(block => block.Text));
            text = WhitespacePattern().Replace(text, " ").Trim();

            return text.Length > 0
                ? text
                : blocks.Any(block => block.IsVideo) ? "Video included" : string.Empty;
        }

        public static bool TryGetYouTubeVideoId(string? value, out string videoId)
        {
            videoId = string.Empty;

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return false;
            }

            var host = uri.Host.ToLowerInvariant();
            if (host.StartsWith("www.", StringComparison.Ordinal))
                host = host[4..];
            if (host.StartsWith("m.", StringComparison.Ordinal))
                host = host[2..];

            string? candidate = null;
            var pathSegments = uri.AbsolutePath
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (host == "youtu.be")
            {
                candidate = pathSegments.FirstOrDefault();
            }
            else if (host is "youtube.com" or "youtube-nocookie.com")
            {
                if (pathSegments.Length > 0 && pathSegments[0].Equals("watch", StringComparison.OrdinalIgnoreCase))
                {
                    candidate = GetQueryValue(uri.Query, "v");
                }
                else if (pathSegments.Length >= 2 &&
                         (pathSegments[0].Equals("shorts", StringComparison.OrdinalIgnoreCase) ||
                          pathSegments[0].Equals("embed", StringComparison.OrdinalIgnoreCase) ||
                          pathSegments[0].Equals("live", StringComparison.OrdinalIgnoreCase)))
                {
                    candidate = pathSegments[1];
                }
            }

            if (candidate == null || !VideoIdPattern().IsMatch(candidate))
                return false;

            videoId = candidate;
            return true;
        }

        private static string ToPlainText(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var withLineBreaks = HtmlBreakPattern().Replace(content, "\n");
            var withoutTags = HtmlTagPattern().Replace(withLineBreaks, string.Empty);
            return WebUtility.HtmlDecode(withoutTags).Replace("\r\n", "\n", StringComparison.Ordinal);
        }

        private static void AddText(List<PostContentBlock> blocks, string text)
        {
            if (text.Length > 0)
                blocks.Add(new PostContentBlock(text, null));
        }

        private static string? GetQueryValue(string query, string key)
        {
            foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = pair.Split('=', 2);
                if (parts.Length == 2 && Uri.UnescapeDataString(parts[0]).Equals(key, StringComparison.OrdinalIgnoreCase))
                    return Uri.UnescapeDataString(parts[1]);
            }

            return null;
        }

        [GeneratedRegex(@"https?://[^\s<>""']+", RegexOptions.IgnoreCase)]
        private static partial Regex UrlPattern();

        [GeneratedRegex(@"^[A-Za-z0-9_-]{11}$")]
        private static partial Regex VideoIdPattern();

        [GeneratedRegex(@"<(?:br\s*/?|/p|/div|/li)>\s*", RegexOptions.IgnoreCase)]
        private static partial Regex HtmlBreakPattern();

        [GeneratedRegex(@"<[^>]+>")]
        private static partial Regex HtmlTagPattern();

        [GeneratedRegex(@"\s+")]
        private static partial Regex WhitespacePattern();
    }
}

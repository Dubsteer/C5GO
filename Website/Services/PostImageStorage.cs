using Microsoft.AspNetCore.Http;

namespace Website.Services
{
    public abstract class ImageStorageBase
    {
        public const long MaximumFileSize = 5 * 1024 * 1024;

        private readonly string uploadDirectory;
        private readonly string publicPath;

        protected ImageStorageBase(
            IWebHostEnvironment environment,
            string folderName)
        {
            uploadDirectory = Path.Combine(
                environment.WebRootPath,
                "Images",
                folderName);
            publicPath = $"/images/{folderName}";
        }

        public async Task<string> SaveAsync(
            IFormFile image,
            CancellationToken cancellationToken = default)
        {
            if (image.Length == 0)
                throw new ImageUploadException("Choose a non-empty image file.");

            if (image.Length > MaximumFileSize)
                throw new ImageUploadException("The image must not exceed 5 MB.");

            var signature = new byte[12];
            int bytesRead;
            await using (var signatureStream = image.OpenReadStream())
            {
                bytesRead = await signatureStream.ReadAsync(signature, cancellationToken);
            }

            var extension = DetectExtension(signature.AsSpan(0, bytesRead));
            if (extension == null)
            {
                throw new ImageUploadException(
                    "Only JPEG, PNG and WebP image files are allowed.");
            }

            Directory.CreateDirectory(uploadDirectory);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            await using var target = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true);

            await image.CopyToAsync(target, cancellationToken);
            return $"{publicPath}/{fileName}";
        }

        public void Delete(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) ||
                !path.StartsWith($"{publicPath}/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var fileName = Path.GetFileName(path);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var filePath = Path.Combine(uploadDirectory, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private static string? DetectExtension(ReadOnlySpan<byte> signature)
        {
            if (signature.Length >= 3 &&
                signature[0] == 0xFF &&
                signature[1] == 0xD8 &&
                signature[2] == 0xFF)
            {
                return ".jpg";
            }

            ReadOnlySpan<byte> pngSignature =
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
            if (signature.StartsWith(pngSignature))
                return ".png";

            if (signature.Length >= 12 &&
                signature[..4].SequenceEqual("RIFF"u8) &&
                signature.Slice(8, 4).SequenceEqual("WEBP"u8))
            {
                return ".webp";
            }

            return null;
        }
    }

    public sealed class PostImageStorage : ImageStorageBase
    {
        public PostImageStorage(IWebHostEnvironment environment)
            : base(environment, "posts")
        {
        }
    }

    public sealed class CommunityImageStorage : ImageStorageBase
    {
        public CommunityImageStorage(IWebHostEnvironment environment)
            : base(environment, "community")
        {
        }
    }

    public sealed class ImageUploadException : Exception
    {
        public ImageUploadException(string message) : base(message)
        {
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Website.Services;

namespace Unit_Tests
{
    [TestClass]
    public class TestPostImageStorage
    {
        private string webRoot = null!;
        private PostImageStorage storage = null!;

        [TestInitialize]
        public void Setup()
        {
            webRoot = Path.Combine(
                Path.GetTempPath(),
                $"c5go-image-tests-{Guid.NewGuid():N}");
            storage = new PostImageStorage(new TestWebHostEnvironment(webRoot));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(webRoot))
                Directory.Delete(webRoot, recursive: true);
        }

        [TestMethod]
        public async Task TestSaveAcceptsPngSignatureAndUsesSafeFileName()
        {
            byte[] content =
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
            var image = CreateFile(content, "unsafe-name.pdf");

            var path = await storage.SaveAsync(image);

            Assert.IsTrue(path.StartsWith("/images/posts/", StringComparison.Ordinal));
            Assert.IsTrue(path.EndsWith(".png", StringComparison.Ordinal));
            Assert.IsTrue(File.Exists(Path.Combine(
                webRoot,
                "Images",
                "posts",
                Path.GetFileName(path))));
        }

        [TestMethod]
        public async Task TestSaveRejectsPdfContentWithImageFileName()
        {
            var image = CreateFile("%PDF-1.7"u8.ToArray(), "photo.png");

            await Assert.ThrowsExactlyAsync<ImageUploadException>(() =>
                storage.SaveAsync(image));
        }

        private static FormFile CreateFile(byte[] content, string fileName)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "Image", fileName);
        }

        private sealed class TestWebHostEnvironment : IWebHostEnvironment
        {
            public TestWebHostEnvironment(string webRootPath)
            {
                WebRootPath = webRootPath;
                ContentRootPath = webRootPath;
            }

            public string ApplicationName { get; set; } = "C5GO.Tests";
            public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
            public string WebRootPath { get; set; }
            public string EnvironmentName { get; set; } = "Development";
            public string ContentRootPath { get; set; }
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }
    }
}

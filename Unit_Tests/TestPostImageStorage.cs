using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Website.Services;

namespace Unit_Tests
{
    [TestClass]
    public class TestPostImageStorage
    {
        private string storageRoot = null!;
        private PostImageStorage storage = null!;

        [TestInitialize]
        public void Setup()
        {
            storageRoot = Path.Combine(
                Path.GetTempPath(),
                $"c5go-image-tests-{Guid.NewGuid():N}");
            storage = new PostImageStorage(new ImageStoragePaths(storageRoot));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(storageRoot))
                Directory.Delete(storageRoot, recursive: true);
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
                storageRoot,
                "posts",
                Path.GetFileName(path))));
        }

        [TestMethod]
        public async Task TestDeleteRemovesOnlyOwnedPostImage()
        {
            byte[] content =
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
            var path = await storage.SaveAsync(CreateFile(content, "post.png"));
            var filePath = Path.Combine(
                storageRoot,
                "posts",
                Path.GetFileName(path));

            storage.Delete($"/images/community/{Path.GetFileName(path)}");
            Assert.IsTrue(File.Exists(filePath));

            storage.Delete(path);
            Assert.IsFalse(File.Exists(filePath));
        }

        [TestMethod]
        public async Task TestSaveRejectsPdfContentWithImageFileName()
        {
            var image = CreateFile("%PDF-1.7"u8.ToArray(), "photo.png");

            await Assert.ThrowsExactlyAsync<ImageUploadException>(() =>
                storage.SaveAsync(image));
        }

        [TestMethod]
        public void TestDevelopmentUsesExistingWebRootImagesByDefault()
        {
            var environment = new TestWebHostEnvironment(storageRoot);

            var result = ImageStoragePathResolver.Resolve(environment, null);

            Assert.AreEqual(
                Path.GetFullPath(Path.Combine(storageRoot, "Images")),
                result);
        }

        [TestMethod]
        public void TestProductionRequiresAbsoluteConfiguredStoragePath()
        {
            var environment = new TestWebHostEnvironment(storageRoot)
            {
                EnvironmentName = Environments.Production
            };

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                ImageStoragePathResolver.Resolve(environment, null));
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                ImageStoragePathResolver.Resolve(environment, "relative/uploads"));

            var result = ImageStoragePathResolver.Resolve(
                environment,
                storageRoot);
            Assert.AreEqual(Path.GetFullPath(storageRoot), result);
        }

        [TestMethod]
        public void TestStoragePathsRejectDirectoryTraversal()
        {
            var paths = new ImageStoragePaths(storageRoot);

            Assert.ThrowsExactly<ArgumentException>(() =>
                paths.GetDirectory("../outside"));
            Assert.ThrowsExactly<ArgumentException>(() =>
                paths.GetDirectory(".."));
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

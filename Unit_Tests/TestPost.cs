using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestPost
    {
        private PostManager postManager = null!;
        private MockPostRepo mockPostRepo = null!;

        [TestInitialize]
        public void TestInit()
        {
            mockPostRepo = new MockPostRepo();
            postManager = new PostManager(mockPostRepo);
        }

        [TestMethod]
        public void TestCreatePost()
        {
            var post = CreatePost();

            postManager.AddPost(post);

            Assert.AreEqual(1, mockPostRepo.GetAllPosts().Count);
            Assert.AreSame(post, mockPostRepo.GetAllPosts()[0]);
        }

        [TestMethod]
        public void TestUpdatePost()
        {
            var post = CreatePost();
            postManager.AddPost(post);
            var updatedPost = new Post(
                post.Id,
                post.User,
                "Updated title",
                "Updated content",
                post.Posted_on);

            postManager.UpdatePost(updatedPost);

            Assert.AreEqual("Updated title", mockPostRepo.GetAllPosts()[0].Title);
            Assert.AreEqual("Updated content", mockPostRepo.GetAllPosts()[0].Content);
        }

        [TestMethod]
        public void TestCreatePostNormalizesTitleAndContent()
        {
            var post = new Post(1, new User(1), "  Test title  ", "  Test content  ", DateTime.Now);

            postManager.AddPost(post);

            Assert.AreEqual("Test title", post.Title);
            Assert.AreEqual("Test content", post.Content);
        }

        [TestMethod]
        public void TestCreatePostRejectsBlankTitle()
        {
            var post = new Post(1, new User(1), "   ", "Test content", DateTime.Now);

            Assert.ThrowsExactly<ArgumentException>(() => postManager.AddPost(post));
            Assert.AreEqual(0, mockPostRepo.GetAllPosts().Count);
        }

        [TestMethod]
        public void TestCreatePostRejectsOversizedTitle()
        {
            var post = new Post(
                1,
                new User(1),
                new string('a', PostManager.MaxTitleLength + 1),
                "Test content",
                DateTime.Now);

            Assert.ThrowsExactly<ArgumentException>(() => postManager.AddPost(post));
            Assert.AreEqual(0, mockPostRepo.GetAllPosts().Count);
        }

        [TestMethod]
        public void TestUpdatePostRejectsBlankContent()
        {
            var existingPost = CreatePost();
            postManager.AddPost(existingPost);
            var invalidPost = new Post(
                existingPost.Id,
                existingPost.User,
                "Valid title",
                "   ",
                existingPost.Posted_on);

            Assert.ThrowsExactly<ArgumentException>(() => postManager.UpdatePost(invalidPost));
            Assert.AreSame(existingPost, mockPostRepo.GetAllPosts()[0]);
        }

        [TestMethod]
        public void TestDeletePost()
        {
            var post = CreatePost();
            postManager.AddPost(post);

            postManager.DeletePost(post);

            Assert.AreEqual(0, mockPostRepo.GetAllPosts().Count);
        }

        [TestMethod]
        public void TestGetPostById()
        {
            var post = CreatePost();
            postManager.AddPost(post);

            Assert.AreSame(post, postManager.GetPostById(post.Id));
            Assert.IsNull(postManager.GetPostById(999));
        }

        private static Post CreatePost() =>
            new Post(1, new User(1), "Test title", "Test content", DateTime.Now);
    }
}

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
            post.Content = "Updated Content";

            postManager.UpdatePost(post);

            Assert.AreEqual("Updated Content", mockPostRepo.GetAllPosts()[0].Content);
        }

        [TestMethod]
        public void TestDeletePost()
        {
            var post = CreatePost();
            postManager.AddPost(post);

            postManager.DeletePost(post);

            Assert.AreEqual(0, mockPostRepo.GetAllPosts().Count);
        }

        private static Post CreatePost() =>
            new Post(1, new User(1), "Test title", "Test content", DateTime.Now);
    }
}

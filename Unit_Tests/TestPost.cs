using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Models;
using LogicLayer.Managers;
using Unit_Tests.MockRepos;
using System;

namespace Unit_Tests
{
    [TestClass]
    public class TestPost
    {
        private PostManager postManager;
        private MockPostRepo mockPostRepo;

        [TestInitialize]
        public void TestInit()
        {
            // Instantiate the mock repository and the manager
            mockPostRepo = new MockPostRepo();
            postManager = new PostManager(mockPostRepo);
        }

        [TestMethod]
        public void TestCreatePost()
        {
            // Arrange
            var user = new User(); // This needs to be replaced with actual User object.
            var post = new Post(1, user, "Test Content", DateTime.Now);

            // Act
            postManager.CreatePost(post);

            // Assert
            Assert.AreEqual(1, mockPostRepo.GetAllPosts().Count);
            Assert.AreEqual(post, mockPostRepo.GetAllPosts()[0]);
        }

        [TestMethod]
        public void TestUpdatePost()
        {
            // Arrange
            var user = new User(); // This needs to be replaced with actual User object.
            var post = new Post(1, user, "Test Content", DateTime.Now);
            postManager.CreatePost(post);

            // Act
            post.Content = "Updated Content";
            postManager.UpdatePost(post);

            // Assert
            Assert.AreEqual(1, mockPostRepo.GetAllPosts().Count);
            Assert.AreEqual("Updated Content", mockPostRepo.GetAllPosts()[0].Content);
        }

        [TestMethod]
        public void TestDeletePost()
        {
            // Arrange
            var user = new User(); // This needs to be replaced with actual User object.
            var post = new Post(1, user, "Test Content", DateTime.Now);
            postManager.CreatePost(post);

            // Act
            postManager.DeletePost(post);

            // Assert
            Assert.AreEqual(0, mockPostRepo.GetAllPosts().Count);
        }

        // More tests can be added here...
    }
}
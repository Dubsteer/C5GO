using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Models;
using LogicLayer.Managers;
using Unit_Tests.MockRepos;
using System.Collections.Generic;
using System.Linq;

namespace Unit_Tests
{
    [TestClass]
    public class TestComment
    {
        private readonly CommentManager commentManager;
        private readonly User testUser;

        public TestComment()
        {
            var commentRepo = new MockCommentRepo();
            commentManager = new CommentManager(commentRepo);
            testUser = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
        }

        [TestMethod]
        public void AddCommentTest()
        {
            var comment = new Comment(null, testUser, "This is a test comment", DateTime.Now, 1);
            commentManager.AddComment(comment);
            var allComments = commentManager.GetAllComments();
            Assert.AreEqual(1, allComments.Count);
            Assert.AreEqual("This is a test comment", allComments.First().Content);
        }

        [TestMethod]
        public void DeleteCommentTest()
        {
            var comment = new Comment(null, testUser, "This is a test comment", DateTime.Now, 1);
            commentManager.AddComment(comment);
            commentManager.DeleteComment(comment);
            var allComments = commentManager.GetAllComments();
            Assert.AreEqual(0, allComments.Count);
        }

        [TestMethod]
        public void UpdateCommentTest()
        {
            var comment = new Comment(null, testUser, "This is a test comment", DateTime.Now, 1);
            commentManager.AddComment(comment);
            comment.Content = "Updated content";
            commentManager.UpdateComment(comment);
            var allComments = commentManager.GetAllComments();
            Assert.AreEqual("Updated content", allComments.First().Content);
        }
    }
}
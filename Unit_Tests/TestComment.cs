using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestComment
    {
        private CommentManager commentManager = null!;
        private User testUser = null!;

        [TestInitialize]
        public void Setup()
        {
            commentManager = new CommentManager(new MockCommentRepo());
            testUser = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "test@example.com", "123", false);
        }

        [TestMethod]
        public void AddCommentTest()
        {
            var comment = CreateComment();

            commentManager.AddComment(comment);
            var allComments = commentManager.GetAllCommentsByPostId(1);

            Assert.AreEqual(1, allComments.Count);
            Assert.AreEqual("This is a test comment", allComments[0].Content);
        }

        [TestMethod]
        public void DeleteCommentTest()
        {
            var comment = CreateComment();
            commentManager.AddComment(comment);

            commentManager.DeleteComment(comment);

            Assert.AreEqual(0, commentManager.GetAllCommentsByPostId(1).Count);
        }

        [TestMethod]
        public void GetCommentWithReplyTest()
        {
            var comment = CreateComment();
            commentManager.AddComment(comment);
            commentManager.AddReply(new CommentReply(1, "Test reply", DateTime.Now, comment.Id, testUser));

            var comments = commentManager.GetAllCommentsWithReplies(1);

            Assert.AreEqual(1, comments.Count);
            Assert.AreEqual(1, comments[0].Replies.Count);
            Assert.AreEqual("Test reply", comments[0].Replies[0].Content);
        }

        private Comment CreateComment() =>
            new Comment(1, testUser, "This is a test comment", DateTime.Now, 1);
    }
}

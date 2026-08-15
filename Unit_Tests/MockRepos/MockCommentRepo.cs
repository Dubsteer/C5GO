using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockCommentRepo : ICommentRepo
    {
        private readonly List<Comment> comments = new();
        private readonly List<CommentReply> replies = new();

        public void AddComment(Comment comment) => comments.Add(comment);

        public List<Comment> GetAllCommentsByPostId(int id) =>
            comments.Where(c => c.PostId == id).ToList();

        public void DeleteComment(Comment comment) => comments.Remove(comment);

        public Comment GetCommentById(int id) =>
            comments.FirstOrDefault(c => c.Id == id)!;

        public void AddReply(CommentReply reply) => replies.Add(reply);

        public List<CommentReply> GetAllRepliesByCommentId(int commentId) =>
            replies.Where(r => r.CommentId == commentId).ToList();

        public CommentReply GetReplyById(int replyId) =>
            replies.FirstOrDefault(r => r.Id == replyId)!;

        public void DeleteReply(CommentReply reply) => replies.Remove(reply);
    }
}

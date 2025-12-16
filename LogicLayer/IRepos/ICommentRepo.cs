using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface ICommentRepo
    {
        // COMMENTS
        void AddComment(Comment comment);
        List<Comment> GetAllCommentsByPostId(int id);
        Comment GetCommentByUserId(int id);
        void UpdateComment(Comment comment);
        void DeleteComment(Comment comment);
        Comment GetCommentById(int id);
        List<Comment> GetAllComments();

        // REPLIES
        void AddReply(CommentReply reply);
        List<CommentReply> GetAllRepliesByCommentId(int commentId);

        // ✅ NEW (needed for delete reply feature)
        CommentReply GetReplyById(int replyId);
        void DeleteReply(CommentReply reply);

        bool CheckIfCommentExists(string commentText);
    }
}

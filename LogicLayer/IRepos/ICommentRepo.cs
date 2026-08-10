using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface ICommentRepo
    {
        void AddComment(Comment comment);
        List<Comment> GetAllCommentsByPostId(int id);
        Comment? GetCommentByUserId(int id);
        void UpdateComment(Comment comment);
        void DeleteComment(Comment comment);
        Comment? GetCommentById(int id);
        List<Comment> GetAllComments();

        void AddReply(CommentReply reply);
        List<CommentReply> GetAllRepliesByCommentId(int commentId);

        CommentReply? GetReplyById(int replyId);
        void DeleteReply(CommentReply reply);

        bool CheckIfCommentExists(string commentText);
    }
}

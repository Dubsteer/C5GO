using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface ICommentRepo
    {
        void AddComment(Comment comment);
        List<Comment> GetAllCommentsByPostId(int id);
        void DeleteComment(Comment comment);
        Comment? GetCommentById(int id);

        void AddReply(CommentReply reply);
        List<CommentReply> GetAllRepliesByCommentId(int commentId);

        CommentReply? GetReplyById(int replyId);
        void DeleteReply(CommentReply reply);
    }
}

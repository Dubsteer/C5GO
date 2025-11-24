using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface ICommentRepo
    {
        void AddComment(Comment comment);

        List<Comment> GetAllCommentsByPostId(int id);

        Comment GetCommentByUserId(int id);

        void UpdateComment(Comment comment);

        void DeleteComment(Comment comment);

        List<Comment> GetAllComments();

        Comment GetCommentById(int id);

        void AddReply(CommentReply reply);

        List<CommentReply> GetAllRepliesByCommentId(int commentId);

        bool CheckIfCommentExists(string comment);
    }
}

using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface ICommentRepo
    {
        public void AddComment(Comment comment);

        public List<Comment> GetAllCommentsByPostId( int id);

        public Comment GetCommentByUserId(int id);

        public void UpdateComment(Comment comment);

        public void DeleteComment(Comment comment);

        public List<Comment> GetAllComments();

        public Comment GetCommentById(int id);

        public void AddReply(CommentReply reply);
        public List<CommentReply> GetAllRepliesByCommentId(int commentId);
        public bool CheckIfCommentExists(string comment);
    }
}

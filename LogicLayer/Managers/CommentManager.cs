using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Exceptions;

namespace LogicLayer.Managers
{
    public class CommentManager
    {
        private readonly ICommentRepo commentRepo;
        private readonly Comment commentId;
        private readonly int replyId;
        private readonly object updatedReply;
        private readonly object comment;

        public CommentManager(ICommentRepo commentRepo)
        {
            this.commentRepo = commentRepo;
        }

        public void AddComment(Comment comment)
        {
            
            commentRepo.AddComment(comment);
        }

        public List<Comment> GetAllCommentsByPostId( int id)
        {
            var comments = commentRepo.GetAllCommentsByPostId(id);
            foreach (var comment in comments)
            {
                var commentReplies = GetAllRepliesByCommentId((int)comment.Id);
                comment.Replies = commentReplies;
            }
            return comments;
        }

        public Comment GetCommentByUserId(int id)
        {
            return commentRepo.GetCommentByUserId(id);
        }

        public void UpdateComment(Comment comment)
        {
            commentRepo.UpdateComment(comment);
        }

        public void DeleteComment(Comment comment)
        {
            commentRepo.DeleteComment(comment);
        }

        public Comment GetCommentById(int id)
        {
            return commentRepo.GetCommentById(id);
        }

        public List<Comment> GetAllComments()
        {
            return commentRepo.GetAllComments();
        }

        public void AddReply(CommentReply reply)
        {
            commentRepo.AddReply(reply);
        }

        public List<CommentReply> GetAllRepliesByCommentId(int commentId)
        {
            return commentRepo.GetAllRepliesByCommentId(commentId);
        }
    }
}


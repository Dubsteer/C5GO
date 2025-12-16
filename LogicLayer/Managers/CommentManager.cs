using LogicLayer.IRepos;
using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.Managers
{
    public class CommentManager
    {
        private readonly ICommentRepo repo;

        public CommentManager(ICommentRepo repo)
        {
            this.repo = repo;
        }

        // COMMENTS
        public void AddComment(Comment c) => repo.AddComment(c);
        public void DeleteComment(Comment c) => repo.DeleteComment(c);
        public Comment GetCommentById(int id) => repo.GetCommentById(id);
        public List<Comment> GetAllCommentsByPostId(int postId) => repo.GetAllCommentsByPostId(postId);

        public List<Comment> GetAllCommentsWithReplies(int postId)
        {
            var list = repo.GetAllCommentsByPostId(postId);

            foreach (var c in list)
            {
                c.Replies = repo.GetAllRepliesByCommentId(c.Id);
            }

            return list;
        }

        // REPLIES
        public void AddReply(CommentReply reply) => repo.AddReply(reply);

        // ✅ NEW
        public CommentReply GetReplyById(int replyId) => repo.GetReplyById(replyId);
        public void DeleteReply(CommentReply reply) => repo.DeleteReply(reply);
    }
}

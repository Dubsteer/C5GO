using LogicLayer.IRepos;
using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.Managers
{
    public class CommentManager
    {
        public const int MaxContentLength = 1500;

        private readonly ICommentRepo repo;

        public CommentManager(ICommentRepo repo)
        {
            this.repo = repo;
        }

        public void AddComment(Comment comment)
        {
            comment.Content = NormalizeContent(comment.Content);
            repo.AddComment(comment);
        }

        public void DeleteComment(Comment c) => repo.DeleteComment(c);
        public Comment? GetCommentById(int id) => repo.GetCommentById(id);
        public List<Comment> GetAllCommentsWithReplies(int postId)
        {
            var list = repo.GetAllCommentsByPostId(postId);

            foreach (var c in list)
            {
                c.Replies = repo.GetAllRepliesByCommentId(c.Id);
            }

            return list;
        }

        public void AddReply(CommentReply reply)
        {
            reply.Content = NormalizeContent(reply.Content);
            repo.AddReply(reply);
        }

        public CommentReply? GetReplyById(int replyId) => repo.GetReplyById(replyId);
        public void DeleteReply(CommentReply reply) => repo.DeleteReply(reply);

        private static string NormalizeContent(string? content)
        {
            var normalized = content?.Trim() ?? string.Empty;

            if (normalized.Length == 0)
                throw new ArgumentException("Comment cannot be empty.", nameof(content));

            if (normalized.Length > MaxContentLength)
                throw new ArgumentException($"Comment cannot exceed {MaxContentLength} characters.", nameof(content));

            return normalized;
        }
    }
}

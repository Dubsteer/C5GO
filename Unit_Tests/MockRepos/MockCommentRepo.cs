using System;
using System.Collections.Generic;
using System.Linq;
using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockCommentRepo : ICommentRepo
    {
        private List<Comment> comments = new List<Comment>();
        private List<CommentReply> replies = new List<CommentReply>();

        public void AddComment(Comment comment)
        {
            comments.Add(comment);
        }

        public bool CheckIfCommentExists(string content)
        {
            return comments.Any(c => c.Content == content);
        }

        public void DeleteComment(Comment comment)
        {
            comments.Remove(comment);
        }

        public void DeleteCommentReply(CommentReply commentReply)
        {
            replies.Remove(commentReply);
        }

        public List<Comment> GetAllComments()
        {
            return comments;
        }

        public List<Comment> GetAllCommentsByPostId(int id)
        {
            return comments.Where(c => c.PostId == id).ToList();
        }

        public List<CommentReply> GetAllReplies()
        {
            return replies;
        }

        public Comment GetCommentById(int id)
        {
            return comments.FirstOrDefault(c => c.Id == id);
        }

        public Comment GetCommentByUserId(int id)
        {
            // Here you need to use the User's ID from the Comment. Update this as per your User object structure.
            return comments.FirstOrDefault(c => c.User.Id == id);
        }

        public void UpdateComment(Comment comment)
        {
            var existingComment = comments.FirstOrDefault(c => c.Id == comment.Id);
            if (existingComment != null)
            {
                existingComment.Content = comment.Content;
                existingComment.User = comment.User;
                existingComment.Posted_on = comment.Posted_on;
                existingComment.Replies = comment.Replies;
                existingComment.PostId = comment.PostId;
            }
        }

        public void UpdateCommentReply(CommentReply commentReply)
        {
            var existingReply = replies.FirstOrDefault(r => r.Id == commentReply.Id);
            if (existingReply != null)
            {
                existingReply.Content = commentReply.Content;
                existingReply.User = commentReply.User;
                existingReply.Posted_on = commentReply.Posted_on;
                existingReply.Replies = commentReply.Replies;
                existingReply.CommentId = commentReply.CommentId;
            }
        }
    }
}
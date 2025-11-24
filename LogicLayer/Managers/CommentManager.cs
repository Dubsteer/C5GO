using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using System;
using System.Collections.Generic;

namespace LogicLayer.Managers
{
    public class CommentManager
    {
        private readonly ICommentRepo _commentRepo;

        public CommentManager(ICommentRepo commentRepo)
        {
            _commentRepo = commentRepo;
        }

        // ---------------------------------------------------------------
        // ADD COMMENT
        // ---------------------------------------------------------------
        public void AddComment(Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content))
                throw new Exception("Comment cannot be empty.");

            // Optional: Prevent duplicate spam
            if (_commentRepo.CheckIfCommentExists(comment.Content))
                throw new CommentAlreadyInUserExpetion("You already posted this comment.");

            _commentRepo.AddComment(comment);
        }

        // ---------------------------------------------------------------
        // GET ALL COMMENTS FOR A POST
        // ---------------------------------------------------------------
        public List<Comment> GetAllCommentsByPostId(int id)
        {
            var comments = _commentRepo.GetAllCommentsByPostId(id);

            // Load replies for each comment
            foreach (var c in comments)
            {
                c.Replies = _commentRepo.GetAllRepliesByCommentId(c.Id.Value);
            }

            return comments;
        }

        // ---------------------------------------------------------------
        // GET COMMENT BY USER ID (legacy)
        // ---------------------------------------------------------------
        public Comment GetCommentByUserId(int userId)
        {
            return _commentRepo.GetCommentByUserId(userId);
        }

        // ---------------------------------------------------------------
        // GET ONE COMMENT
        // ---------------------------------------------------------------
        public Comment GetCommentById(int id)
        {
            return _commentRepo.GetCommentById(id);
        }

        // ---------------------------------------------------------------
        // UPDATE COMMENT
        // ---------------------------------------------------------------
        public void UpdateComment(Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content))
                throw new Exception("Comment cannot be empty.");

            _commentRepo.UpdateComment(comment);
        }

        // ---------------------------------------------------------------
        // DELETE COMMENT
        // ---------------------------------------------------------------
        public void DeleteComment(Comment comment)
        {
            if (comment == null || comment.Id == null)
                throw new Exception("Comment does not exist.");

            _commentRepo.DeleteComment(comment);
        }

        // ---------------------------------------------------------------
        // ADD REPLY
        // ---------------------------------------------------------------
        public void AddReply(CommentReply reply)
        {
            if (string.IsNullOrWhiteSpace(reply.Content))
                throw new Exception("Reply cannot be empty.");

            _commentRepo.AddReply(reply);
        }

        // ---------------------------------------------------------------
        // GET ALL
        // ---------------------------------------------------------------
        public List<Comment> GetAllComments()
        {
            return _commentRepo.GetAllComments();
        }

        public List<CommentReply> GetAllRepliesByCommentId(int commentId)
        {
            return _commentRepo.GetAllRepliesByCommentId(commentId);
        }
    }
}

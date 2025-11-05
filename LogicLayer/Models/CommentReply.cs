using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Models
{
    public class CommentReply : Comment
    {
        private int replyId;
        private DateTime postedOn;

        public int CommentId { get; set; }
        public CommentReply(int id, User user, string content, DateTime posted_on, int commentId) : base(id, user, content, posted_on, 0)
        {
            Id = id;
            User = user;
            Content = content;
            Posted_on = posted_on;
            CommentId = commentId;
        }

        public CommentReply(int id, User user, string content, DateTime posted_on, int commentId, List<CommentReply> replies) : base(id, user, content, posted_on, 0)
        {
            Id = id;
            User = user;
            Content = content;
            Posted_on = posted_on;
            CommentId = commentId;
            Replies = replies;
        }

        public CommentReply(int replyId, string content, DateTime postedOn, int commentId)
        {
            this.replyId = replyId;
            Content = content;
            this.postedOn = postedOn;
            CommentId = commentId;
        }

        public CommentReply(User? currentUser, string replyText, DateTime now, int parentCommentId)
        {
        }
    }
}
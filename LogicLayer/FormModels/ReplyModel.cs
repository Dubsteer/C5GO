using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.FormModels
{
    public class ReplyModel
    {
       // [Required(ErrorMessage = "Please enter your reply.")]
        [MaxLength(150, ErrorMessage = "Comment reply should not exceed 150 characters.")]
        public string ReplyText { get; set; }

        public int replyCommentId { get; set; }
    }
}

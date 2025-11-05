using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.FormModels
{
    public class CommentModel
    {
       // [Required(ErrorMessage = "Please enter your comment.")]
        [MaxLength(150, ErrorMessage = "Comment should not exceed 150 characters.")]
        public string CommentText { get; set; }

        public int commentPostId { get; set; }
    }
}

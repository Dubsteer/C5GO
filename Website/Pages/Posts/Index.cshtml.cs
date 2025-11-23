using LogicLayer.Models;
using LogicLayer.Managers;
using LogicLayer.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using LogicLayer.FormModels;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Reflection.Metadata;
using Microsoft.IdentityModel.Tokens;

namespace Website.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly UserManager userManager;
        private readonly CommentManager commentManager;

        public Post post { get; set; }
        public List<Comment> comments { get; set; }
        public User currentUser { get; set; }

        [BindProperty]
        public CommentModel NewComment { get; set; }

        [BindProperty]
        public ReplyModel NewReply { get; set; }

        public IndexModel(PostManager postManager, UserManager userManager, CommentManager commentManager)
        {
            this.postManager = postManager;
            this.userManager = userManager;
            this.commentManager = commentManager;
        }

        public IActionResult OnGet(int id)
        {
            post = postManager.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            comments = commentManager.GetAllCommentsByPostId(id);

            if (User.Identity.IsAuthenticated)
            {
                currentUser = userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value));
            }
            else
            {
                currentUser = null;
            }

            return Page();
        }

        public IActionResult OnPostSubmitComment()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            currentUser = userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value));

            post = postManager.GetPostById(NewComment.commentPostId);
            comments = commentManager.GetAllCommentsByPostId((int)post.Id);
            
            Comment comment = new Comment(
                null,
                currentUser,
                NewComment.CommentText,
                DateTime.Now,
                NewComment.commentPostId
            );

            try
            {
                commentManager.AddComment(comment);
            }
            catch (CommentAlreadyInUserExpetion ex)
            {
                ViewData["Error"] = ex.Message;
                Debug.WriteLine(ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500); // Internal server error
            }
            

            return RedirectToPage("Index", new { id = post.Id });
        }

        public IActionResult OnPostSubmitReply()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            currentUser = userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value));

            var comment = commentManager.GetCommentById(NewReply.replyCommentId);
            post = postManager.GetPostById(comment.PostId);
            comments = commentManager.GetAllCommentsByPostId((int)post.Id);

            
                CommentReply reply = new CommentReply(
                    0,
                    NewReply.ReplyText,
                    DateTime.Now,
                    NewReply.replyCommentId
                );

                try
                {
                    commentManager.AddReply(reply);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return StatusCode(500); // Internal server error
                }
            

            return RedirectToPage("Index", new { id = post.Id });
        }

        public IActionResult OnPostDeleteComment(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            currentUser = userManager.GetUserById(
                Convert.ToInt32(User.FindFirst("id").Value)
            );

            var comment = commentManager.GetCommentById(id);
            if (comment == null)
                return NotFound();

            post = postManager.GetPostById(comment.PostId);

            // ? user who wrote comment OR admin can delete
            if (comment.User.Id == currentUser.Id || currentUser.IsAdmin)
            {
                commentManager.DeleteComment(comment);
                return RedirectToPage("Index", new { id = post.Id });
            }

            return Unauthorized();
        }
    }
}
using System.Security.Claims;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Unit_Tests.MockRepos;
using Website.Pages.Posts;

namespace Unit_Tests
{
    [TestClass]
    public class TestPostPage
    {
        [TestMethod]
        public void AjaxCommentReturnsUpdatedPartialInsteadOfRedirect()
        {
            var context = CreateContext();
            context.Model.NewComment = new CommentModel { CommentText = "New comment" };

            var result = context.Model.OnPostSubmitComment();

            var partial = Assert.IsInstanceOfType<PartialViewResult>(result);
            Assert.AreEqual("_CommentList", partial.ViewName);
            Assert.AreEqual("New comment", context.CommentRepo.GetAllCommentsByPostId(1).Single().Content);
        }

        [TestMethod]
        public void ReplyCannotTargetCommentFromAnotherPost()
        {
            var context = CreateContext();
            context.CommentRepo.AddComment(new Comment(
                7,
                context.CurrentUser,
                "Another post",
                DateTime.Now,
                2));
            context.Model.NewReply = new ReplyModel { CommentId = 7, ReplyText = "Invalid reply" };

            var result = context.Model.OnPostSubmitReply();

            Assert.IsInstanceOfType<NotFoundObjectResult>(result);
            Assert.AreEqual(0, context.CommentRepo.GetAllRepliesByCommentId(7).Count);
        }

        [TestMethod]
        public void UserCannotDeleteAnotherUsersComment()
        {
            var context = CreateContext();
            var otherUser = new User(2, "Other", "User", 20, "other", "other@example.com", "hash", false);
            context.CommentRepo.AddComment(new Comment(8, otherUser, "Keep this", DateTime.Now, 1));

            var result = context.Model.OnPostDeleteComment(8);

            var forbidden = Assert.IsInstanceOfType<ObjectResult>(result);
            Assert.AreEqual(StatusCodes.Status403Forbidden, forbidden.StatusCode);
            Assert.IsNotNull(context.CommentRepo.GetCommentById(8));
        }

        private static TestContext CreateContext()
        {
            var currentUser = new User(1, "Test", "User", 22, "tester", "test@example.com", "hash", false);
            var userManager = new UserManager(new MockUserRepo([currentUser]));
            var postManager = new PostManager(new MockPostRepo());
            var commentRepo = new MockCommentRepo();
            var commentManager = new CommentManager(commentRepo);

            postManager.AddPost(new Post(1, currentUser, "Test post", "Post content", DateTime.Now));

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim("id", "1"), new Claim(ClaimTypes.Name, currentUser.Username)],
                    "TestAuthentication"))
            };
            httpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

            var pageContext = new PageContext
            {
                HttpContext = httpContext,
                ViewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
            };

            var model = new PostModel(postManager, userManager, commentManager)
            {
                Id = 1,
                PageContext = pageContext
            };

            return new TestContext(model, commentRepo, currentUser);
        }

        private sealed record TestContext(
            PostModel Model,
            MockCommentRepo CommentRepo,
            User CurrentUser);
    }
}

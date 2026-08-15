using LogicLayer.IRepos;
using LogicLayer.Models;

namespace LogicLayer.Managers
{
    public class PostManager
    {
        public const int MaxTitleLength = 200;

        private readonly IPostRepo repo;

        public PostManager(IPostRepo repo)
        {
            this.repo = repo;
        }

        public void AddPost(Post post)
        {
            NormalizeAndValidate(post);
            repo.CreatePost(post);
        }

        public void UpdatePost(Post post)
        {
            NormalizeAndValidate(post);
            repo.UpdatePost(post);
        }

        public void DeletePost(Post post)
        {
            repo.DeletePost(post);
        }

        public Post? GetPostById(int id)
        {
            return repo.GetPostById(id);
        }

        public List<Post> GetAllPosts()
        {
            return repo.GetAllPosts();
        }

        private static void NormalizeAndValidate(Post post)
        {
            ArgumentNullException.ThrowIfNull(post);

            post.Title = post.Title?.Trim() ?? string.Empty;
            post.Content = post.Content?.Trim() ?? string.Empty;

            if (post.Title.Length == 0)
                throw new ArgumentException("Title is required.", nameof(post.Title));

            if (post.Title.Length > MaxTitleLength)
                throw new ArgumentException($"Title cannot exceed {MaxTitleLength} characters.", nameof(post.Title));

            if (post.Content.Length == 0)
                throw new ArgumentException("Content is required.", nameof(post.Content));
        }
    }
}

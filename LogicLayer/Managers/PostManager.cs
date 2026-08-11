using LogicLayer.IRepos;
using LogicLayer.Models;

namespace LogicLayer.Managers
{
    public class PostManager
    {
        private readonly IPostRepo repo;

        public PostManager(IPostRepo repo)
        {
            this.repo = repo;
        }

        public void AddPost(Post post)
        {
            repo.CreatePost(post);
        }

        public void UpdatePost(Post post)
        {
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
    }
}

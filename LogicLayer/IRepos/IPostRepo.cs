using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface IPostRepo
    {
        public void CreatePost(Post post);

        public List<Post> GetAllPosts();

        public Post? GetPostById(int id);

        public void UpdatePost(Post post);

        public void DeletePost(Post post);
    }
}

using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface IPostRepo
    {
        public void CreatePost(Post post);

        public List<Post> GetAllPosts();

        public void UpdatePost(Post post);

        public void DeletePost(Post post);

        public bool CheckIfPostNameExists(string postName, int selfId);
    }
}

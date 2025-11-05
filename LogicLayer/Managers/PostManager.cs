using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using System.Diagnostics;

namespace LogicLayer.Managers
{
    public class PostManager
    {
        private readonly IPostRepo postRepo;
        public PostManager(IPostRepo postRepo)
        {
            this.postRepo = postRepo;
        }

        public void CreatePost(Post post)
        {
            if (postRepo.CheckIfPostNameExists(post.Content, post.Id.GetValueOrDefault()))
                throw new PostNameAlreadyInUseExepction("That post  is already in use. Please change it and try again.");

            postRepo.CreatePost(post);
        }

        public List<Post> GetAllPosts() 
        {
            return postRepo.GetAllPosts();
        }

        public Post? GetPostById(int id)
        {
            return postRepo.GetAllPosts().FirstOrDefault(p => p.Id.Value == id);
        }

        public void UpdatePost(Post post)
        {
            if (postRepo.CheckIfPostNameExists(post.Content, post.Id.GetValueOrDefault()))
                throw new PostNameAlreadyInUseExepction("That post is already in use. Please change it and try again.");
            
            postRepo.UpdatePost(post);
        }

        public void DeletePost(Post post)
        {
            postRepo.DeletePost(post);
        }
    }
}

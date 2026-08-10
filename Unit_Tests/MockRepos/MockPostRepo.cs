using System;
using System.Collections.Generic;
using System.Linq;
using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockPostRepo : IPostRepo
    {
        private List<Post> posts = new();

        public bool CheckIfPostNameExists(string postName, int selfId)
        {
            return posts.Any(p => p.Content == postName && p.Id != selfId);
        }

        public void CreatePost(Post post)
        {
            posts.Add(post);
        }

        public void DeletePost(Post post)
        {
            posts.RemoveAll(p => p.Id == post.Id);
        }

        public List<Post> GetAllPosts()
        {
            return new List<Post>(posts);
        }

        public void UpdatePost(Post post)
        {
            var existingPost = posts.Find(p => p.Id == post.Id);

            if (existingPost != null)
            {
                existingPost.Content = post.Content;
            }
            else
            {
                throw new Exception("Post not found");
            }
        }
    }
}

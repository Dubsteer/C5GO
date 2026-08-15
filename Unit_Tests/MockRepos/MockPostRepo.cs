using System;
using System.Collections.Generic;
using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockPostRepo : IPostRepo
    {
        private readonly List<Post> posts = new();

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

        public Post? GetPostById(int id)
        {
            return posts.Find(post => post.Id == id);
        }

        public void UpdatePost(Post post)
        {
            var index = posts.FindIndex(p => p.Id == post.Id);

            if (index < 0)
                throw new Exception("Post not found");

            posts[index] = post;
        }
    }
}

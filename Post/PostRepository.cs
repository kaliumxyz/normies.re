﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NormiesRe.Models;

namespace NormiesRe.Post
{
    public interface IPostRepository
    {
        IReadOnlyCollection<Models.Post> GetLatestPosts(int amount);
        void SavePost(Models.Post post);
        Models.Post FindPostById(int id);
        void DeletePostById(int id);
        void SaveComment(Comment comment);
    }

    public class PostRepository : IPostRepository
    {
        private readonly NormiesDbContext dbContext;

        public PostRepository(NormiesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IReadOnlyCollection<Models.Post> GetLatestPosts(int amount)
        {
            var posts = dbContext.Posts
                .OrderByDescending(p => p.ReleaseDate)
                .Take(25)
                .ToImmutableList();

            return posts;
        }

        public void SavePost(Models.Post post)
        {
            dbContext.Add(post);
            dbContext.SaveChanges();
        }

        public Models.Post FindPostById(int id)
        {
            return dbContext.Posts
                .Include( p => p.Comments)
                .FirstOrDefault(p => p.ID == id);
        }

        public void DeletePostById(int id)
        {
            dbContext.Posts.Remove(FindPostById(id));
            dbContext.SaveChanges();
        }
        
        public void SaveComment(Comment comment)
        {
            dbContext.Add(comment);
            dbContext.SaveChanges();
        }
    }
}

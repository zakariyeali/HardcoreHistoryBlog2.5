﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HardcoreHistoryBlog.Data;
using HardcoreHistoryBlog.Models;
using HardcoreHistoryBlog.Models.Blog_Models;
using HardcoreHistoryBlog.Models.Comments;
using HardcoreHistoryBlog.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HardcoreHistoryBlog.Core
{
    public interface IRepository
    {
        Post GetPost(int id);
        List<Post> GetAllPosts();
        List<Post> GetAllPosts(string Category);

        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(int id);
        void AddSubComment(SubComment comment);


        Task<bool> SaveChangesAsync();
    }
}




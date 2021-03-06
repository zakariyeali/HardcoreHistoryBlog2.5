﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardcoreHistoryBlog.Core;
using HardcoreHistoryBlog.Models.Blog_Models;
using Microsoft.AspNetCore.Mvc;
using HardcoreHistoryBlog.Data;
using Microsoft.AspNetCore.Authorization;
using HardcoreHistoryBlog.Models.Comments;
using HardcoreHistoryBlog.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace HardcoreHistoryBlog.Controllers
{

    public class PostsController : Controller
    {

        private IRepository _repo;
        private IFileManager _fileManager;
        UserManager<ApplicationUser> _userManager;

        public PostsController( 
            IFileManager fileManager,
            IRepository repo,
            UserManager<ApplicationUser> userManager
            )
        {
            _fileManager = fileManager;
            _repo = repo;
            _userManager = userManager;

        }

        [AutoValidateAntiforgeryToken]
        public IActionResult Index(string category)
        {
            var posts = string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category);
            // if no categories then get all posts, otherwise get all posts BY category
            return View(posts);
        }
        //Get single post
        [AutoValidateAntiforgeryToken]
        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);
            return View(post);
        }

        [HttpGet("/Image/{image}")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.')+1);
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

        [Authorize(Roles ="Customer, Admin")]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = vm.PostId });
            }

            var post = _repo.GetPost(vm.PostId);
            if (vm.MainCommentId == 0)
            {
                var user = await _userManager.GetUserAsync(User);
                var email = user.Email;
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                    Edited = DateTime.Now,
                    By = user.FirstName,
                    UserId = user.Id 
                });
                _repo.UpdatePost(post);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var email = user.Email;
                var comment = new SubComment
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                    Edited = DateTime.Now,
                    By = user.FirstName,
                    UserId = user.Id
                };
                _repo.AddSubComment(comment);
            }
            await _repo.SaveChangesAsync();
            return RedirectToAction("Post", new { id = vm.PostId });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> RemoveComment (int id)
        {
            _repo.RemoveComment(id);
            await _repo.SaveChangesAsync();
            return RedirectToAction("Post");
        }

        

    }
}
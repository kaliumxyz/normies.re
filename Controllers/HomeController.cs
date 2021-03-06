﻿using System;
using Microsoft.AspNetCore.Mvc;
using NormiesRe.Post;

namespace NormiesRe.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostListService postListService;
        private readonly INewPostService newPostService;
        private readonly IPostFindService postFindService;
        private readonly IPostDeleteService postDeleteService;
        private readonly INewCommentService newCommentService;

        public HomeController(IPostListService postListService, 
            INewPostService newPostService, 
            IPostFindService postFindService, 
            IPostDeleteService postDeleteService,
            INewCommentService newCommentService)
        {
            this.postListService = postListService;
            this.newPostService = newPostService;
            this.postFindService = postFindService;
            this.postDeleteService = postDeleteService;
            this.newCommentService = newCommentService;
        }
        
        [Route("")]
        public IActionResult Index()
        {
            return View(postListService.GetNewestPosts());
        }

        [Route("/new")]
        public IActionResult New()
        {
            return View();
        }

        [Route("/show/{id}")]
        public IActionResult Show(int id)
        {
            var viewModel = postFindService.FindPostById(id);
            if (viewModel == null)
            {
                return Redirect("/");
            }

            return View(viewModel);
        }

        [Route("/delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (Environment.GetEnvironmentVariable("DELETE_KEY") == Request.Query["key"])
            {
                postDeleteService.DeletePostById(id);
            }

            return Redirect("/");
        }

        [HttpPost]
        [Route("/addpost")]
        public IActionResult AddPost([Bind("Title,Content")] NewPostFormModel newPostFormModel)
        {
            // fuck off 
            if (newPostFormModel.Title.Trim() == "" || 
                newPostFormModel.Content.Trim() == "" || 
                newPostFormModel.Content.Length > 20000 || newPostFormModel.Title.Length > 200)
            {
                return Redirect("/");
            }
            
            newPostService.AddPostByFormModel(newPostFormModel);
            return Redirect("/");
        }
        
        [HttpPost]
        [Route("/addcomment/{postid}")]
        public IActionResult AddComment(int postid, [Bind("Content")] NewCommentFormModel newPostFormModel)
        {
            if (newPostFormModel.Content.Trim() == "" || 
                newPostFormModel.Content.Length > 20000)
            {
                return Redirect("/");
            }

            var post = postFindService.FindPostById(postid);
            if (post == null)
            {
                return Redirect("/");
            }
            
            newCommentService.AddCommentToPost(postid, newPostFormModel);
            return Redirect($"/show/{postid}");
        }
    }
}

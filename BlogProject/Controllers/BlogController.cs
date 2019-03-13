using BlogProject.Models;
using BlogProject.Models.Domain;
using BlogProject.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.Controllers
{
    public class BlogController : Controller
    {
        private ApplicationDbContext DbContext;

        public BlogController()
        {
            DbContext = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var model = DbContext.AllPosts

            .Select(p => new IndexPostBlogViewModel
            {
                PostId = p.Id,
                Title = p.Title,
                Body = p.Body.Substring(0, 50),
                Published = p.Published,
                DateCreated = p.DateCreated,
                DateUpdated = p.DateUpdated,
                ApplicationUser = p.ApplicationUser

            }).ToList();

            return View(model);
        }
        
        public ActionResult AllPosts()
        {          
            var model = DbContext.AllPosts

            .Select(p => new IndexPostBlogViewModel
            {   
                PostId = p.Id,                
                Title = p.Title,
                Body = p.Body,
                Published = p.Published,
                DateCreated = p.DateCreated,
                DateUpdated = p.DateUpdated,
                ApplicationUser = p.ApplicationUser

            }).ToList();

            return View(model); 
        }

        public ActionResult Register()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Post()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Post(PostEditViewModel formData)
        {
            return SavePost(null, formData);
        }

        private ActionResult SavePost(int? id, PostEditViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }          
            
            //string fileExtension;

            ////Validating file upload
            //if (formData.Media != null)
            //{
            //    fileExtension = Path.GetExtension(formData.Media.FileName);

            //    if (!Constants.AllowedFileExtensions.Contains(fileExtension))
            //    {
            //        ModelState.AddModelError("", "File extension is not allowed.");                    
            //        return View();
            //    }
            //}

            Post myPost;

            if (!id.HasValue)
            {
                myPost = new Post()
                {
                    Title = formData.Title,
                    Body = formData.Body,
                    Published = formData.Published,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                };                
                DbContext.AllPosts.Add(myPost);
            }
            else
            {
                myPost = DbContext.AllPosts.FirstOrDefault(p => p.Id == id);

                if (myPost == null)
                {
                    return RedirectToAction(nameof(BlogController.AllPosts));
                }
            }

            myPost.Title = formData.Title ;
            myPost.Body = formData.Body;
            myPost.Published = formData.Published;
            myPost.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();
            return RedirectToAction(nameof(BlogController.AllPosts));

            //Handling file upload
            //if (formData.Media != null)
            //{
            //    if (!Directory.Exists(Constants.MappedUploadFolder))
            //    {
            //        Directory.CreateDirectory(Constants.MappedUploadFolder);
            //    }

            //    var fileName = formData.Media.FileName;
            //    var fullPathWithName = Constants.MappedUploadFolder + fileName;

            //    formData.Media.SaveAs(fullPathWithName);

            //    movie.MediaUrl = Constants.UploadFolder + fileName;
            //}

            //DbContext.SaveChanges();

        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.AllPosts));
            }

            var post = DbContext.AllPosts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return RedirectToAction(nameof(BlogController.AllPosts));
            }

            var model = new IndexPostBlogViewModel
            {
                PostId = post.Id,
                Title = post.Title,
                Body = post.Body,
                Published = post.Published,
                DateCreated = post.DateCreated,
                DateUpdated = post.DateUpdated,
                ApplicationUser = post.ApplicationUser

            };


            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.AllPosts));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.AllPosts.FirstOrDefault(
                p => p.Id == id);

            if (post == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = new PostEditViewModel();
            model.Title = post.Title;
            model.Body = post.Body;            
            model.Published = post.Published;

            DbContext.SaveChanges();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, PostEditViewModel formData)
        {
            return SavePost(id, formData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.AllPosts));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.AllPosts.FirstOrDefault(p => p.Id == id);

            if (post != null)
            {
                DbContext.AllPosts.Remove(post);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(BlogController.AllPosts));
        }
    }
}
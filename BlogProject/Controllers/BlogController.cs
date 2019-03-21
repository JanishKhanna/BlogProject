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

            myPost.Title = formData.Title;
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
                return RedirectToAction(nameof(BlogController.Index));
            }

            var post = DbContext.AllPosts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = new PostDetailsViewModel
            {

                PostId = post.Id,
                Title = post.Title,
                Body = post.Body,
                Published = post.Published,
                DateCreated = post.DateCreated,
                DateUpdated = post.DateUpdated,
                //ApplicationUser = post.ApplicationUser
                AllComments = post.AllComments.Select(p => new CommentViewModel()
                {
                    CommentId = p.Id,
                    Body = p.Body,
                    DateCreated = p.DateCreated,
                    DateUpdated = p.DateUpdated,
                    UpdatedReason = p.UpdatedReason,
                    ApplicationUser = p.ApplicationUser

                }).ToList()
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
                post.AllComments.Clear();
                DbContext.AllPosts.Remove(post);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(BlogController.AllPosts));
        }

        [HttpGet]
        [Route("Blog/{name}")]
        public ActionResult DetailsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name.ToLower()))
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.AllPosts.FirstOrDefault(p =>
            p.Title == name);

            if (post == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = new PostDetailsViewModel();
            model.Title = post.Title;
            model.Body = post.Body;
            model.Published = post.Published;
            model.DateCreated = post.DateCreated;
            //model.MediaUrl = movie.MediaUrl;

            return View("Details", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateComments(int id, CreateEditCommentViewModel formData)
        {
            formData.PostId = id;
            return SaveComments(null, formData);
        }

        private ActionResult SaveComments(int? id, CreateEditCommentViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Comments myComment;

            if (!id.HasValue)
            {
                var userId = User.Identity.GetUserId();
                var post = DbContext.AllPosts.FirstOrDefault(p => p.Id == formData.PostId);

                if (post == null)
                {
                    return RedirectToAction(nameof(BlogController.Index));
                }

                myComment = new Comments()
                {
                    DateCreated = DateTime.Now,
                    ApplicationUser = DbContext.Users.FirstOrDefault(p => p.Id == userId),
                };

                DbContext.AllComments.Add(myComment);
                post.AllComments.Add(myComment);
            }
            else
            {
                myComment = DbContext.AllComments.FirstOrDefault(p => p.Id == id);

                if (myComment == null)
                {
                    return RedirectToAction(nameof(BlogController.Details));
                }
                myComment.UpdatedReason = formData.UpdatedReason;
            }

            myComment.Body = formData.Body;
            myComment.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();
            return RedirectToAction(nameof(BlogController.Details));
        }

        [HttpGet]
        [Authorize(Roles = "Admin , Moderator")]
        public ActionResult EditComment(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Details));
            }

            var userId = User.Identity.GetUserId();

            var myComment = DbContext.AllComments
                .FirstOrDefault(p => p.Id == id);

            if (myComment == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = new CreateEditCommentViewModel();
            model.Body = myComment.Body;
            model.UpdatedReason = myComment.UpdatedReason;
            model.DateUpdated = myComment.DateUpdated;

            DbContext.SaveChanges();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditComment(int id, CreateEditCommentViewModel formData)
        {
            return SaveComments(id, formData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin , Moderator")]
        public ActionResult DeleteComment(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Details));
            }

            var userId = User.Identity.GetUserId();

            var myComment = DbContext.AllComments
                .FirstOrDefault(p => p.Id == id);

            if (myComment != null)
            {
                DbContext.AllComments.Remove(myComment);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(BlogController.Details));
        }

        [HttpPost]
        public ActionResult Search(string myString)
        {
            var mySearch = myString;
            var model = DbContext.AllPosts
                .Where(p => p.Title.Contains(mySearch) || p.Body.Contains(mySearch))
                .Select(p => new IndexPostBlogViewModel
                {
                    Title = p.Title,
                    Body = p.Body                   

                }).ToList();
            return View(model);
        }        
    }
}
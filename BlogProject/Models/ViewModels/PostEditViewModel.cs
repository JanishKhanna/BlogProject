using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.Models.ViewModels
{
    public class PostEditViewModel
    {
        [Required]
        public string Title { get; set; }        

        [AllowHtml]
        [Required]
        public string Body { get; set; }

        [Required]
        public bool Published { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public HttpPostedFileBase Media { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.Models.ViewModels
{
    public class CreateEditCommentViewModel
    {   
        public int PostId { get; set; }
        
        [Required]
        public string Body { get; set; }
        
        public string UpdatedReason { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }        
    }
}
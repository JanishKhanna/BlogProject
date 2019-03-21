using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogProject.Models.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string UpdatedReason { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
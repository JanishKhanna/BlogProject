using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogProject.Models.ViewModels
{
    public class IndexPostBlogViewModel
    {        
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public ApplicationUser ApplicationUser { get; set; }        
    }
}
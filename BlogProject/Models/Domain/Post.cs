﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogProject.Models.Domain
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; } 
        public virtual List<Comments> AllComments { get; set; }
    }
}
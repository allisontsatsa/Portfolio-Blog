using PagedList;
using Portfolio_Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio_Blog.ViewModels
{
    public class LandingPageViewModel
    {
        public Blog Blog { get; set; }
        public List<Blog> OtherBlogs { get; set; }

        public LandingPageViewModel()
        {
            OtherBlogs = new List<Blog>();
        }
    }
}
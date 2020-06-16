using Portfolio_Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio_Blog.Helpers
{
    public class SearchHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public IQueryable<Blog> IndexSearch(string searchStr)
        {
            IQueryable<Blog> result = null;
            if (searchStr != null)
            {
                result = db.Blogs.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                p.Body.Contains(searchStr) || p.Comments.Any(c =>
                c.Body.Contains(searchStr) || c.Author.FirstName.Contains(searchStr) ||
                c.Author.LastName.Contains(searchStr) ||
                c.Author.DisplayName.Contains(searchStr) ||
                c.Author.Email.Contains(searchStr)));
            }
            else { result = db.Blogs.AsQueryable(); }

            return result.OrderByDescending(p => p.Created);
        }
    }
}
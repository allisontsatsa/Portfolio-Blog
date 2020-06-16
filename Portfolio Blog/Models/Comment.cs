using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portfolio_Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string AuthorId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdateReason { get; set; }

        [AllowHtml]
        public string Body { get; set; }
        public virtual Blog Blog { get; set; }
        public virtual ApplicationUser Author {get; set;}
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portfolio_Blog.Helpers;
using Portfolio_Blog.Models;
using PagedList;
using PagedList.Mvc;
using Portfolio_Blog.ViewModels;

namespace Portfolio_Blog.Controllers

{
    [RequireHttps]
    [Authorize(Roles = "Admin")]
    public class BlogsController : Controller

    {
        
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: Blogs
        public ActionResult Index(int? page)
        {
            int pageSize = 5; //Display 3 blog posts at a time
            int pageNumber = (page ?? 1);
            return View(db.Blogs.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
        }

        // GET: Blogs/Details/5
        [AllowAnonymous]
        public ActionResult Details(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blogPost = db.Blogs.FirstOrDefault(b => b.Slug == slug);
          
            if (blogPost == null)
            {
                return HttpNotFound();
            }

            //I am writing a LINQ statement that gets all published Blogs that aren't the main blogPost
            //thats why I have the && b.Id != blogPost.Id
            //This list of Other Blogs is used as my source...
            var randomBlogSource = db.Blogs.AsNoTracking().Where(b => b.Published && b.Id != blogPost.Id).ToList();

            var model = new LandingPageViewModel();
            model.Blog = blogPost;

            //This is how I create Random numbers in C#, I create a variable of type Random and then call the Next() method. 
            //The arguments for Next() are the lower bound inclusive and an upper bound exclusive (i.e. rand.Next(0, 10)) will
            //ghenerate and return a number between 0 and 9
            var rand = new Random();
            var randIndex = -1;
            var randomBlogSourceCount = -1;

            //This For Loop is here because I want to make 3 random selections out of the otherblogs list so that I can show them on the bottom of the Details view
            for (var loop = 0; loop <= 2; loop++)
            {
                //Here I am determining the upper bound for the Next method which is why I am calling the Count method
                randomBlogSourceCount = randomBlogSource.Count;

                //randIndex is an integer between 0 and otherblogs.Count - 1
                randIndex = rand.Next(0, randomBlogSourceCount);

                //I am adding to the OtherBlogs property of the LandingPageViewModel random Blogs from the Source
                model.OtherBlogs.Add(randomBlogSource[randIndex]);

                //Then I remove the Blog I found from the Source so it isn't chosen again
                randomBlogSource.RemoveAt(randIndex);
            }
            return View(model);
        }

        // GET: Blogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Abstract,Body,MediaUrl,Published")] Blog blog, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blog.Title); ;
                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blog);
                }
                if (db.Blogs.Any(p => p.Slug == slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blog);
                }
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var justFileName = Path.GetFileNameWithoutExtension(image.FileName);
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    justFileName = $"{justFileName}-{DateTime.Now.Ticks}";

                    justFileName = $"{justFileName}{Path.GetExtension(image.FileName)}";

                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), justFileName));
                    blog.MediaUrl = "/Uploads/" + justFileName;
                }

                blog.Slug = slug;
                blog.Created = DateTime.Now;
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Created,Abstract,Slug,Body,MediaUrl,Published")] Blog blog, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blog.Title);
                if (blog.Slug != slug)
                {
                    if (string.IsNullOrEmpty(slug))
                    {
                        ModelState.AddModelError("Title", "Oops, looks like you forgot a title!");
                        return View(blog);
                    }
                    if (db.Blogs.Any(b => b.Slug == slug))
                    {
                        ModelState.AddModelError("", $"Oops, the title '{blog.Title}' has been used before.");
                    }
                    blog.Slug = slug;

                    if (ImageUploadValidator.IsWebFriendlyImage(image))
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));

                        blog.MediaUrl = "/Uploads/" + fileName;
                    }
                }
                blog.Updated = DateTime.Now;
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

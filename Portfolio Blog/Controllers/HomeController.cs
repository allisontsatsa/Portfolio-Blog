using Portfolio_Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Portfolio_Blog.Helpers;

namespace Portfolio_Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private SearchHelper searchHelper = new SearchHelper();
        public ActionResult Index(int? page, string searchStr)

        {
            ViewBag.Search = searchStr;
            var blogList = searchHelper.IndexSearch(searchStr);
            int pageSize = 5; // the number of posts you want to display per page             
            int pageNumber = (page ?? 1);

            var blogs = blogList.ToPagedList(pageNumber, pageSize);
            return View(blogs);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            Email model = new Email();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(Email model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                    var from = $"Allison Tsatsa's Portfolio<{WebConfigurationManager.AppSettings["emailfrom"]}>";
                    model.Body = "This is a message from your portoflio site. The name and email of the contacting person is above.";
                    var email = new MailMessage(from, WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Website Contact",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };
                    var svc = new EmailService();
                    await svc.SendAsync(email);

                    return View(new Email());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);

                }
            }
            return View(model);
        }

    }
}
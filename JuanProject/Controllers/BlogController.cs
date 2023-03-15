using JuanProject.DAL;
using JuanProject.Models;
using JuanProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JuanProject.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {

            ViewBag.UserId = null;

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.UserId = user.Id;
            }

            BlogDetailVM blogDetailVM = new()
            {

                Blog = _context.Blogs
                .Include(b=>b.Comments)
                .ThenInclude(c=>c.AppUser)
                .FirstOrDefault(b => b.Id == id),
                Blogs = _context.Blogs.OrderByDescending(b => b.Id).Take(3).ToList()
            };

            return View(blogDetailVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string commentMessage,int blogId)
        {
            AppUser user = null;
            if (User.Identity.IsAuthenticated)
            {
                 user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
            Comment comment = new();
            comment.CreatedDate= DateTime.Now;
            comment.AppUserId = user.Id;
            comment.BlogId = blogId;
            comment.Message= commentMessage;
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("detail", new {id=blogId});
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = _context.Comments.FirstOrDefault(b=>b.Id== id);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return RedirectToAction("detail", new {id=comment.BlogId});
        }
    }
}


using InterLinked.Data;
using InterLinked.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InterLinked.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<InterlinkedAppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(
            UserManager<InterlinkedAppUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Post
                .Include(p => p.User)
                .Where(p => p.ValidTo == null || p.ValidTo > DateTime.Now)
                .OrderByDescending(p => p.PostedAt)
                .ToListAsync();

            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
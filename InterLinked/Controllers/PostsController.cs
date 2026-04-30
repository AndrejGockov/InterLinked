using InterLinked.Data;
using InterLinked.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InterLinked.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<InterlinkedAppUser> _userManager;

        public PostsController(ApplicationDbContext context,
        UserManager<InterlinkedAppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Post
                .Include(p => p.User)
                .ToListAsync();

            return View(posts);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var post = await _context.Post
                .Include(p => p.User)
                .Include(p => p.Applications)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            bool canViewApplicants =
                user != null &&
                post.UserId == user.Id &&
                user.organizationType != InterlinkedAppUser.UserType.Personal;

            ViewBag.CanViewApplicants = canViewApplicants;

            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Company")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Create(Post post, string ValidToDate)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            post.UserId = userId;
            post.PostedAt = DateTime.Now;

            // ✅ SAFE: date only, auto 23:59
            if (!string.IsNullOrWhiteSpace(ValidToDate) &&
                DateTime.TryParse(ValidToDate, out var date))
            {
                post.ValidTo = new DateTime(
                    date.Year,
                    date.Month,
                    date.Day,
                    23, 59, 0
                );
            }

            // validation
            if (post.ValidTo.HasValue && post.ValidTo < post.PostedAt)
            {
                ModelState.AddModelError("ValidTo", "ValidTo can't be in the past");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ValidToDate = ValidToDate;
                return View(post);
            }

            _context.Post.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyPosts));
        }

        // GET: Edit
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var post = await _context.Post.FindAsync(id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,WebsiteLink")] Post post, string ValidToDate)
        {
            if (id != post.PostId)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var existingPost = await _context.Post.FindAsync(id);

            if (existingPost == null)
                return NotFound();

            if (existingPost.UserId != userId)
                return Forbid();

            if (!string.IsNullOrWhiteSpace(ValidToDate) &&
                DateTime.TryParse(ValidToDate, out var date))
            {
                existingPost.ValidTo = new DateTime(
                    date.Year,
                    date.Month,
                    date.Day,
                    23, 59, 0
                );
            }

            existingPost.Title = post.Title;
            existingPost.Description = post.Description;
            existingPost.WebsiteLink = post.WebsiteLink;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var post = await _context.Post.FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);

            if (post == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            if (post.UserId != userId)
                return Forbid();

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // MY POSTS
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> MyPosts()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            var posts = await _context.Post
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PostedAt)
                .ToListAsync();

            return View(posts);
        }

        // JSON
        [HttpGet]
        public async Task<IActionResult> GetPostsJson()
        {
            var posts = await _context.Post
                .Include(p => p.User)
                .OrderByDescending(p => p.PostedAt)
                .Select(p => new
                {
                    id = p.PostId,
                    title = p.Title,
                    description = p.Description,
                    postedAt = p.PostedAt.ToString("dd.MM.yyyy"),
                    validTo = p.ValidTo.HasValue ? p.ValidTo.Value.ToString("dd.MM.yyyy") : "No Expiration",
                    isActive = !p.ValidTo.HasValue || p.ValidTo > DateTime.Now,
                    companyName = p.User.UserName
                })
                .ToListAsync();

            return Json(new { data = posts });
        }
    }
}
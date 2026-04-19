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
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Post post)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(errors);
            }

            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            post.UserId = userId;
            post.PostedAt = DateTime.Now;

            _context.Post.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyPosts));
        }

    // GET: Posts/Edit/5
    public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,PostedAt,ValidTo,WebsiteLink")] Post post)
        {
            if (id != post.PostId)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var existingPost = await _context.Post.FindAsync(id);

            if (existingPost == null)
                return NotFound();

            if (existingPost.UserId != userId)
                return Forbid();

            if (ModelState.IsValid)
            {
                existingPost.Title = post.Title;
                existingPost.Description = post.Description;
                existingPost.ValidTo = post.ValidTo;
                existingPost.WebsiteLink = post.WebsiteLink;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.PostId == id);
        }




        [Authorize]
        public async Task<IActionResult> MyPosts()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            var posts = await _context.Post
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PostedAt)
                .ToListAsync();

            return View(posts);
        }
    }
}

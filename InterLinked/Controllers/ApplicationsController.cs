using InterLinked.Data;
using InterLinked.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterLinked.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<InterlinkedAppUser> _userManager;

        public ApplicationsController(
            ApplicationDbContext context,
            UserManager<InterlinkedAppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // -------------------------
        // INDEX (ALL APPLICATIONS)
        // -------------------------
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applications = _context.Applications
                .Include(a => a.Post)
                .Include(a => a.User);

            return View(await applications.ToListAsync());
        }

        // -------------------------
        // GET: APPLY PAGE
        // /Applications/Create?postId=5
        // -------------------------
        [Authorize]
        [HttpGet]
        public IActionResult Create(int postId)
        {
            return View(new Application
            {
                PostId = postId
            });
        }

        // -------------------------
        // POST: APPLY ACTION
        // -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Application model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.organizationType != InterlinkedAppUser.UserType.Individual)
                return Forbid();

            // Validate Post exists (IMPORTANT FIX)
            var postExists = await _context.Post.AnyAsync(p => p.PostId == model.PostId);
            if (!postExists)
                return BadRequest("Invalid PostId");

            var alreadyApplied = await _context.Applications
                .AnyAsync(a => a.PostId == model.PostId && a.UserId == user.Id);

            if (alreadyApplied)
                return RedirectToAction(nameof(MyApplications));

            var application = new Application
            {
                PostId = model.PostId,
                UserId = user.Id,
                AppliedAt = DateTime.Now,
                Status = ApplicationStatus.Pending
            };

            _context.Applications.Add(application);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(MyApplications));
        }

        // -------------------------
        // MY APPLICATIONS
        // -------------------------
        [Authorize]
        public async Task<IActionResult> MyApplications()
        {
            var userId = _userManager.GetUserId(User);

            var applications = await _context.Applications
            .Include(a => a.Post)
                .ThenInclude(p => p.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

            return View(applications);
        }

        // -------------------------
        // DELETE APPLICATION
        // -------------------------
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var application = await _context.Applications
                .Include(a => a.Post)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);

            if (application == null)
                return NotFound();

            return View(application);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            if (application.UserId != userId)
                return Forbid();

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyApplications));
        }






        [Authorize]
        public async Task<IActionResult> Approve(int id)
        {
            var application = await _context.Applications
                .Include(a => a.Post)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);

            if (application == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.organizationType == InterlinkedAppUser.UserType.Individual)
                return Forbid();

            // MUST own the post
            if (application.Post?.UserId != user.Id)
                return Forbid();

            application.Status = ApplicationStatus.Approved;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Posts", new { id = application.PostId });
        }

        [Authorize]
        public async Task<IActionResult> Reject(int id)
        {
            var application = await _context.Applications
                .Include(a => a.Post)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);

            if (application == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.organizationType == InterlinkedAppUser.UserType.Individual)
                return Forbid();

            // MUST own the post
            if (application.Post?.UserId != user.Id)
                return Forbid();

            application.Status = ApplicationStatus.Rejected;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Posts", new { id = application.PostId });
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadCv(IFormFile cvFile)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (cvFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(cvFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await cvFile.CopyToAsync(stream);
                }

                user.CvPath = "/uploads/" + fileName;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("MyApplications");
        }

    }
}
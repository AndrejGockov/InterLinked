using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using InterLinked.Data;
using InterLinked.Models;

namespace InterLinked.Controllers;

// ─── Authorization Filter ───────────────────────────────────────────────────

public class OrganizationOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity!.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var orgType = user.FindFirstValue("OrganizationType");

        if (orgType != nameof(InterlinkedAppUser.UserType.Company) &&
            orgType != nameof(InterlinkedAppUser.UserType.StudentOrganization))
        {
            context.Result = new ForbidResult();
        }
    }
}

// ─── Controller ─────────────────────────────────────────────────────────────

public class PostController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<InterlinkedAppUser> _userManager;

    public PostController(ApplicationDbContext context, UserManager<InterlinkedAppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Post — browse all active posts
    public async Task<IActionResult> Index(string? search, string? tag)
    {
        var query = _context.Posts
            .Include(p => p.Company)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Title!.Contains(search) || p.Description!.Contains(search));

        if (!string.IsNullOrEmpty(tag))
            query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Name == tag));

        return View(await query.ToListAsync());
    }

    // GET: /Post/Details/id
    public async Task<IActionResult> Details(Guid id)
    {
        var post = await _context.Posts
            .Include(p => p.Company)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return NotFound();

        return View(post);
    }

    // GET: /Post/Create
    [HttpGet]
    [OrganizationOnly]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Post/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [OrganizationOnly]
    public async Task<IActionResult> Create(Post post, string? tags)
    {
        if (!ModelState.IsValid) return View(post);

        var company = await GetCurrentCompany();
        if (company == null) return Unauthorized();

        post.CompanyId = company.Id;
        post.PostedAt = DateTime.Now;

        await AttachTags(post, tags);

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Post created successfully!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Post/Edit/id
    [HttpGet]
    [OrganizationOnly]
    public async Task<IActionResult> Edit(Guid id)
    {
        var post = await _context.Posts
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return NotFound();
        if (!await IsOwnedByCurrentCompany(post)) return Unauthorized();

        ViewBag.CurrentTags = string.Join(", ", post.PostTags.Select(pt => pt.Tag.Name));
        return View(post);
    }

    // POST: /Post/Edit/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [OrganizationOnly]
    public async Task<IActionResult> Edit(Guid id, Post updated, string? tags)
    {
        var post = await _context.Posts
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return NotFound();
        if (!await IsOwnedByCurrentCompany(post)) return Unauthorized();
        if (!ModelState.IsValid) return View(updated);

        post.Title = updated.Title;
        post.Description = updated.Description;
        post.ValidTo = updated.ValidTo;
        post.IsActive = updated.IsActive;
        post.ThumbnailUrl = updated.ThumbnailUrl;
        post.ApplyExternalLink = updated.ApplyExternalLink;
        post.ApplyInternalLink = updated.ApplyInternalLink;

        post.PostTags.Clear();
        await AttachTags(post, tags);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Post updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Post/ToggleActive/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [OrganizationOnly]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null) return NotFound();
        if (!await IsOwnedByCurrentCompany(post)) return Unauthorized();

        post.IsActive = !post.IsActive;
        await _context.SaveChangesAsync();

        TempData["Success"] = post.IsActive ? "Post reactivated." : "Post deactivated.";
        return RedirectToAction(nameof(Index));
    }

    // ─── Private Helpers ────────────────────────────────────────────────────

    private async Task<Company?> GetCurrentCompany()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return null;

        return await _context.Companies
            .FirstOrDefaultAsync(c => c.UserId == user.Id);
    }

    private async Task<bool> IsOwnedByCurrentCompany(Post post)
    {
        var company = await GetCurrentCompany();
        return company != null && post.CompanyId == company.Id;
    }

    private async Task AttachTags(Post post, string? tags)
    {
        if (string.IsNullOrEmpty(tags)) return;

        foreach (var tagName in tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = tagName.Trim();
            var existing = await _context.Tags.FirstOrDefaultAsync(t => t.Name == trimmed);

            if (existing == null)
            {
                existing = new Tag { Name = trimmed };
                _context.Tags.Add(existing);
            }

            post.PostTags.Add(new PostTag { Tag = existing });
        }
    }
}
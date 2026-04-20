using InterLinked.Data;
using InterLinked.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterLinked.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly UserManager<InterlinkedAppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CompaniesController(UserManager<InterlinkedAppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetCompaniesJson()
        {
            var companies = await _userManager.Users
                .Where(u => u.organizationType == InterlinkedAppUser.UserType.Company)
                .Select(u => new {
                    userName = u.UserName,
                    email = u.Email,
                    phone = u.PhoneNumber ?? "N/A",
                    id = u.Id,
                    profilePicturePath = u.ProfilePicturePath
                })
                .ToListAsync();

            return Json(new { data = companies });
        }

        public async Task<IActionResult> Details(string id)
        {
            var company = await _context.Users
                .Include(u => u.MyPosts)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (company == null) return NotFound();
            return View(company);
        }
    }
}
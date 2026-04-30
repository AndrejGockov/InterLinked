using System.ComponentModel.DataAnnotations;
using System.IO;
using InterLinked.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InterLinked.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<InterlinkedAppUser> _userManager;
        private readonly SignInManager<InterlinkedAppUser> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(
            UserManager<InterlinkedAppUser> userManager,
            SignInManager<InterlinkedAppUser> signInManager,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Company Description")]
            public string? CompanyDescription { get; set; }

            [Display(Name = "Profile Picture")]
            public IFormFile? ProfilePicture { get; set; }

            public InterlinkedAppUser.UserType OrganizationType { get; set; }

            public string? InstagramUrl { get; set; }
            public string? FacebookUrl { get; set; }
            public string? LinkedInUrl { get; set; }
            public string? TwitterUrl { get; set; }
        }

        private async Task LoadAsync(InterlinkedAppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                CompanyDescription = user.CompanyDescription,
                OrganizationType = user.organizationType, // FIX: Load existing type
                InstagramUrl = user.InstagramUrl,
                FacebookUrl = user.FacebookUrl,
                LinkedInUrl = user.LinkedInUrl,
                TwitterUrl = user.TwitterUrl
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // 1. Update basic properties directly to avoid validation loops
            user.PhoneNumber = Input.PhoneNumber;
            user.CompanyDescription = Input.CompanyDescription;
            user.InstagramUrl = Input.InstagramUrl;
            user.FacebookUrl = Input.FacebookUrl;
            user.LinkedInUrl = Input.LinkedInUrl;
            user.TwitterUrl = Input.TwitterUrl;

            // Ensure the required enum is passed back
            user.organizationType = Input.OrganizationType;

            // 2. File Upload Logic
            if (Input.ProfilePicture != null)
            {
                string folder = Path.Combine(_environment.WebRootPath, "uploads/profiles");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, user.ProfilePicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Input.ProfilePicture.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(fileStream);
                }
                user.ProfilePicturePath = "/uploads/profiles/" + fileName;
            }

            // 3. Save
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
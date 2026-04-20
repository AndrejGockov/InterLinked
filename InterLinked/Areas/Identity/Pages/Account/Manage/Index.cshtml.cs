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
            public string PhoneNumber { get; set; }

            [Display(Name = "Company Description")]
            public string CompanyDescription { get; set; }

            [Display(Name = "Profile Picture")]
            public IFormFile ProfilePicture { get; set; }

            [Display(Name = "Instagram")]
            public string InstagramUrl { get; set; }

            [Display(Name = "Facebook")]
            public string FacebookUrl { get; set; }

            [Display(Name = "LinkedIn")]
            public string LinkedInUrl { get; set; }

            [Display(Name = "Twitter / X")]
            public string TwitterUrl { get; set; }
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

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // 1. Update Phone
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
                await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);

            // 2. Update Description
            user.CompanyDescription = Input.CompanyDescription;

            // 3. Update Social Media
            user.InstagramUrl = Input.InstagramUrl;
            user.FacebookUrl = Input.FacebookUrl;
            user.LinkedInUrl = Input.LinkedInUrl;
            user.TwitterUrl = Input.TwitterUrl;

            // 4. Handle File Upload
            if (Input.ProfilePicture != null)
            {
                string folder = Path.Combine(_environment.WebRootPath, "uploads/profiles");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, user.ProfilePicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + Input.ProfilePicture.FileName;
                string filePath = Path.Combine(folder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePicturePath = "/uploads/profiles/" + fileName;
            }

            // 5. Save
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                StatusMessage = "Error updating profile.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
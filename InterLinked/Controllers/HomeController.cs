using InterLinked.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InterLinked.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<InterlinkedAppUser> _userManager;

        public HomeController(UserManager<InterlinkedAppUser> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<IActionResult> IndexAsync()
        {
            var userId = _userManager.GetUserId(User);
            InterlinkedAppUser? user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

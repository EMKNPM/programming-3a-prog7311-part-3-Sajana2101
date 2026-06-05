using GLMS2.Interfaces;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GLMS2.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApiService;

        public AccountController(IAuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var token = await _authApiService.LoginAsync(model);

                if (string.IsNullOrWhiteSpace(token))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Invalid username or password.");

                    return View(model);
                }

                HttpContext.Session.SetString("JwtToken", token);
                TempData["SuccessMessage"] = "You are now logged in.";

                return RedirectToAction("Index", "Contract");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            TempData["SuccessMessage"] = "You have been logged out.";

            return RedirectToAction("Login", "Account");
        }
    }
}
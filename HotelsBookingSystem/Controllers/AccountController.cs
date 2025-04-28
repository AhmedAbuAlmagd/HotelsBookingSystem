using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Results;
using HotelsBookingSystem.Services;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                var (succeeded, errors) = await accountService.RegisterAsync(registerVM);
                if (succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(registerVM);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel userVm)
        {
            if (ModelState.IsValid)
            {
                var result = await accountService.LoginAsync(userVm);
                if (result.Succeeded)
                {
                    return result.IsAdmin ? RedirectToAction("Dashboard", "Admin")
                                          : RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", result.ErrorMessage);
            }

            return View(userVm);
        }

        public async Task<IActionResult> LogoutAsync ()
        {
            await accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

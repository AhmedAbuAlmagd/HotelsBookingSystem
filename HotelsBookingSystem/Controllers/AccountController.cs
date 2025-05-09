using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Results;
using HotelsBookingSystem.Services;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using HotelsBookingSystem.ViewModels.AccountViewModels;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;

namespace HotelsBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration configuration;

        public AccountController(IAccountService accountService , IConfiguration configuration , SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger)
        {
            this.accountService = accountService;
            this.configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
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
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
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



        #region External Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider = "Google", string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            if (provider == "Google")
            {
                properties.Items["prompt"] = "select_account";
            }
            return Challenge(properties, provider);
        }


        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                _logger.LogError("External provider error: {Error}", remoteError);
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", new LoginViewModel());
            }

            var info = await accountService.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Failed to retrieve external login info");
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", new LoginViewModel());
            }

            var result = await accountService.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Provider}", info.LoginProvider);
                var myuser = await accountService.FindByExternalLoginAsync(info.LoginProvider, info.ProviderKey);
                var isAdmin = await _userManager.IsInRoleAsync(myuser, "Admin");
                return isAdmin ? RedirectToAction("Dashboard", "Admin")
                               : RedirectToLocal(returnUrl);
            }

            var user = await accountService.FindByExternalLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
                var firstName = info.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = info.Principal.FindFirst(ClaimTypes.Surname)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError("Email not provided by {Provider}", info.LoginProvider);
                    ModelState.AddModelError(string.Empty, "Email not provided by external provider.");
                    return View("Login", new LoginViewModel());
                }

                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = firstName+' '+lastName
                };

                var createResult = await accountService.CreateExternalUserAsync(newUser, info);
                if (createResult.Succeeded)
                {
                    _logger.LogInformation("New user created with {Provider}", info.LoginProvider);
                    await accountService.SignInAsync(newUser, isPersistent: false);
                    var isAdmin = await _userManager.IsInRoleAsync(newUser, "Admin");
                    return isAdmin ? RedirectToAction("Dashboard", "Admin")
                                   : RedirectToLocal(returnUrl);
                }

                foreach (var error in createResult.Errors)
                {
                    _logger.LogError("User creation error: {Error}", error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                _logger.LogError("User exists but login failed for {Provider}", info.LoginProvider);
                ModelState.AddModelError(string.Empty, "User already exists but login failed.");
            }

            return View("Login", new LoginViewModel());
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion


        public async Task<IActionResult> LogoutAsync ()
        {
            await accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await accountService.FindEmail(model.Email);
            if (user == null)
            {
                TempData["Message"] = "If an account with that email exists, a password reset link has been sent.";
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            try
            {
                var token = await accountService.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, protocol: Request.Scheme);

                var smtpSettings = configuration.GetSection("SmtpSettings");
                using var smtpClient = new SmtpClient(smtpSettings["Host"])
                {
                    Port = int.Parse(smtpSettings["Port"]),
                    Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                    EnableSsl = true,
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                    Subject = "Reset Your Password",
                    IsBodyHtml = true,
                };
                string htmlBody = $@"
                               <html>
                               <body>
                                   <h2>Hotels Booking System - Password Reset</h2>
                                   <p>Hello,</p>
                                   <p>We received a request to reset your password. If this was you, click the link below:</p>
                                   <p><a href='{callbackUrl}'>Reset your password</a></p>
                                   <p>If you didn’t request a password reset, please ignore this email.</p>
                                   <p>Thank you,<br>The Hotels Booking System Team</p>
                               </body>
                               </html>";

                mailMessage.Body = htmlBody;
                mailMessage.To.Add(model.Email);


                await smtpClient.SendMailAsync(mailMessage);
                TempData["Message"] = "If an account with that email exists, a password reset link has been sent.";
            }
            catch (SmtpException ex)
            {
                ModelState.AddModelError("", "An error occurred while sending the password reset email. Please try again later.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Email and token are required.");
            }

            var model = new ResetPasswordViewModel { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await accountService.ResetPasswordAsync(model.Email, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

    }
}

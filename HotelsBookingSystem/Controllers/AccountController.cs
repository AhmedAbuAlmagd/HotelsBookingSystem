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
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

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

                // Check if user with this email already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    // Link the external login to the existing user
                    var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                    if (addLoginResult.Succeeded)
                    {
                        await accountService.SignInAsync(existingUser, isPersistent: false);
                        var isAdmin = await _userManager.IsInRoleAsync(existingUser, "Admin");
                        return isAdmin ? RedirectToAction("Dashboard", "Admin")
                                       : RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        foreach (var error in addLoginResult.Errors)
                        {
                            _logger.LogError("Failed to link external login: {Error}", error.Description);
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View("Login", new LoginViewModel());
                    }
                }

                // If no existing user found, create a new one
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

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(smtpSettings["FromName"], smtpSettings["FromEmail"]));
                email.To.Add(new MailboxAddress("", model.Email));
                email.Subject = "Reset Your Password";

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <style>
                            body {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                line-height: 1.6;
                                color: #333333;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                            }}
                            .header {{
                                background-color: #4a90e2;
                                color: white;
                                padding: 20px;
                                text-align: center;
                                border-radius: 5px 5px 0 0;
                            }}
                            .content {{
                                background-color: #ffffff;
                                padding: 30px;
                                border: 1px solid #e0e0e0;
                                border-radius: 0 0 5px 5px;
                            }}
                            .button {{
                                display: inline-block;
                                background-color: #4a90e2;
                                color: white;
                                text-decoration: none;
                                padding: 12px 24px;
                                border-radius: 5px;
                                margin: 20px 0;
                                font-weight: bold;
                            }}
                            .footer {{
                                text-align: center;
                                margin-top: 20px;
                                color: #666666;
                                font-size: 14px;
                            }}
                            .warning {{
                                background-color: #fff3cd;
                                border: 1px solid #ffeeba;
                                color: #856404;
                                padding: 15px;
                                border-radius: 5px;
                                margin: 20px 0;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Password Reset Request</h1>
                            </div>
                            <div class='content'>
                                <h2>Hello,</h2>
                                <p>We received a request to reset your password for your Hotels Booking System account. If this was you, please click the button below to reset your password:</p>
                                
                                <div style='text-align: center;'>
                                    <a href='{callbackUrl}' class='button'>Reset Password</a>
                                </div>

                                <p>Or copy and paste this link into your browser:</p>
                                <p style='word-break: break-all; color: #4a90e2;'>{callbackUrl}</p>

                                <div class='warning'>
                                    <strong>Note:</strong> This password reset link will expire in 24 hours. If you didn't request a password reset, please ignore this email or contact support if you have concerns.
                                </div>

                                <p>Thank you,<br>
                                <strong>The Hotels Booking System Team</strong></p>
                            </div>
                            <div class='footer'>
                                <p>This is an automated message, please do not reply to this email.</p>
                                <p>&copy; {DateTime.Now.Year} Hotels Booking System. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                email.Body = builder.ToMessageBody();

                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    await smtp.ConnectAsync(smtpSettings["Host"], int.Parse(smtpSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }

                TempData["Message"] = "If an account with that email exists, a password reset link has been sent.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while sending the password reset email. Please try again later.");
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

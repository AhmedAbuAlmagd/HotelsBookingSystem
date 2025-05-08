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

namespace HotelsBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IConfiguration configuration;

        public AccountController(IAccountService accountService , IConfiguration configuration)
        {
            this.accountService = accountService;
            this.configuration = configuration;
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

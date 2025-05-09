using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Results;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HotelsBookingSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        public async Task<LoginResult> LoginAsync(LoginViewModel vm)
        {
            var user = await userManager.FindByNameAsync(vm.UserName);
            if (user == null)
                return new LoginResult { Succeeded = false, ErrorMessage = "Invalid USerName or password." };

            var result = await signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe,false);
            if (!result.Succeeded)
                return new LoginResult { Succeeded = false, ErrorMessage = "Invalid UserName or password." };

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            return new LoginResult
            {
                Succeeded = true,
                User = user,
                IsAdmin = isAdmin
            };
        }

    

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel registerVM)
        {
            var user = new ApplicationUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FullName = registerVM.FullName,
                Country = registerVM.Country,
                City = registerVM.City,
                NationalId = registerVM.NationalId
            };

            var result = await userManager.CreateAsync(user, registerVM.Password);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                await userManager.AddToRoleAsync(user, "User");
                return (true, Enumerable.Empty<string>());
            }

            return (false, result.Errors.Select(e => e.Description));
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser> FindEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user != null)
                return user;
            else 
                return null;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
             return await userManager.GeneratePasswordResetTokenAsync(user);
        }


        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return IdentityResult.Success;
            }

            return await userManager.ResetPasswordAsync(user, token, password);
        }

        #region External Login 
        public async Task<ExternalLoginResult> ProcessExternalLoginAsync(ClaimsPrincipal principal)
        {
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return new ExternalLoginResult
                {
                    Succeeded = false,
                    ErrorMessage = "Email claim not received from the provider."
                };
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email };
                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return new ExternalLoginResult
                    {
                        Succeeded = false,
                        ErrorMessage = string.Join(", ", createResult.Errors.Select(e => e.Description))
                    };
                }
            }

            var loginInfo = await signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return new ExternalLoginResult
                {
                    Succeeded = false,
                    ErrorMessage = "Failed to retrieve external login info."
                };
            }

            // Try to sign in
            var signInResult = await signInManager.ExternalLoginSignInAsync(
                loginInfo.LoginProvider,
                loginInfo.ProviderKey,
                isPersistent: false);

            if (!signInResult.Succeeded)
            {
                // Add the external login if missing
                var addLoginResult = await userManager.AddLoginAsync(user, loginInfo);
                if (addLoginResult.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return new ExternalLoginResult { Succeeded = true, Email = email };
                }
                else
                {
                    return new ExternalLoginResult
                    {
                        Succeeded = false,
                        ErrorMessage = "Failed to link external login."
                    };
                }
            }

            return new ExternalLoginResult { Succeeded = true, Email = email };
        }

        public async Task<IdentityResult> CreateExternalUserAsync(ExternalLoginViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}"
            };

            var result = await userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                await signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }
        #endregion
    }
}

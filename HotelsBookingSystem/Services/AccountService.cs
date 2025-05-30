﻿using HotelsBookingSystem.Models;
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
            try
            {
                return await userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                // Log the exception (assuming you inject a logger into the service)
                // This helps with debugging if the lookup fails due to a database issue
                throw new InvalidOperationException($"Failed to find user by email: {email}", ex);
            }
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
        public async Task<ApplicationUser> FindByExternalLoginAsync(string provider, string providerKey)
        {
            return await userManager.FindByLoginAsync(provider, providerKey);
        }

        public async Task<IdentityResult> CreateExternalUserAsync(ApplicationUser user, ExternalLoginInfo loginInfo)
        {
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return await userManager.AddLoginAsync(user, loginInfo);
            }
            return result;
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> ExternalLoginSignInAsync(string provider, string providerKey, bool isPersistent)
        {
            return await signInManager.ExternalLoginSignInAsync(provider, providerKey, isPersistent, bypassTwoFactor: true);
        }

        public async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            await signInManager.SignInAsync(user, isPersistent);
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return await signInManager.GetExternalLoginInfoAsync();
        }
    
    #endregion
}
}

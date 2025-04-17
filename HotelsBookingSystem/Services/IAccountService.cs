using HotelsBookingSystem.Models;
using HotelsBookingSystem.ViewModels;

namespace HotelsBookingSystem.Services
{
    public interface IAccountService
    {
        Task<(bool Succeeded, string ErrorMessage, ApplicationUser User)> LoginAsync(LoginViewModel loginVM);
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel registerVM);

    }
}

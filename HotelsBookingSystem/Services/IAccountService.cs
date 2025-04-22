using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Results;
using HotelsBookingSystem.ViewModels;

namespace HotelsBookingSystem.Services
{
    public interface IAccountService
    {
        Task<LoginResult> LoginAsync(LoginViewModel loginVM);
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel registerVM);
        Task LogoutAsync();

    }
}

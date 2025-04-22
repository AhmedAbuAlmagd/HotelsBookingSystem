using HotelsBookingSystem.ViewModels.AdminViewModels;

namespace HotelsBookingSystem.Services
{
    public interface IAdminService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();

    }
}

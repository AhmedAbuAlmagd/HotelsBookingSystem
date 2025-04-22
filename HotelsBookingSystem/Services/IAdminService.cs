using HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard;

namespace HotelsBookingSystem.Services
{
    public interface IAdminService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();

    }
}

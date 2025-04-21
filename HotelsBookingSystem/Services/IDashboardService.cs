using HotelsBookingSystem.ViewModels.AdminViewModels;

namespace HotelsBookingSystem.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();

    }
}

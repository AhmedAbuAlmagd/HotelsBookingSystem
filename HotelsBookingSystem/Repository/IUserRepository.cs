using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface IUserRepository
    {
        Task<int> GetTotalUsersCountAsync();
        Task<List<ApplicationUser>> GetTopClientsAsync(int count);
    }
}

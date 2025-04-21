using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetTopClientsAsync(int count);
    }
}

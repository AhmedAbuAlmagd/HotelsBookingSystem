using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId);
    }
}

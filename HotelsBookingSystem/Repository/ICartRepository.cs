using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task AddCartAsync(Cart cart);
        Task AddToCartAsync(CartItem cartItem);
        Task<CartItem> GetCartItemByIdAsync(int id);
        Task DeleteCartItem(CartItem cartItem);

        Task SaveAsync();
    }
}

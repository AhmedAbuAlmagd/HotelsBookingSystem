using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> AddCartAsync(Cart cart);
        Task AddToCartAsync(CartItem cartItem);
        Task<Cart?> GetCartWithItemsAndRoomsByUserIdAsync(string userId);

        Task<CartItem> GetCartItemByIdAsync(int id);
        Task DeleteCartItem(CartItem cartItem);
        Task<decimal> CalculateTotalAmountAsync(int cartId);
        Task ClearCartByUserIdAsync(string userId );
        Task<Cart> GetCartByIdAsync(int cartId);

        Task SaveAsync();

    }
}

using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Repository
{
    public class CartRepository : ICartRepository
    {

        private readonly HotelsContext _context;

        public CartRepository(HotelsContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
        .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Room)
                .ThenInclude(r => r.Hotel)
                    .ThenInclude(h => h.HotelServices)
                        .ThenInclude(hs => hs.Service)
        .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Room)
                .ThenInclude(r => r.RoomImages)
        .FirstOrDefaultAsync(c => c.UserId == userId);
        }


        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();  
        }

        public async Task AddToCartAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync(); 
        }
        public async Task<CartItem> GetCartItemByIdAsync(int id)
        {
            return await _context.CartItems.FindAsync(id);
        }

        public async Task DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync(); 
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync(); 
        }

      
    }
}

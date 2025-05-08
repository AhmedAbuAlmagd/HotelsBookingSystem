using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly HotelsContext _context;

        public CartItemRepository(HotelsContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId)
        {
            return await _context.CartItems.Where(ci => ci.CartId == cartId).ToListAsync();
        }
    }

}

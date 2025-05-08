using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Repository
{
    public class CartRepository : ICartRepository
    {

        private readonly HotelsContext _context;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IRoomRepository _roomRepository;


        public CartRepository(HotelsContext context, ICartItemRepository cartItemRepository, IRoomRepository roomRepository)
        {
            _context = context;
            _cartItemRepository = cartItemRepository;
            _roomRepository = roomRepository;
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
                    .ThenInclude(ci => ci.Room.RoomImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }


        public async Task<decimal> CalculateTotalAmountAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Room)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null || cart.CartItems == null)
                return 0;

            decimal total = 0;

            foreach (var item in cart.CartItems)
            {
                int nights = (item.CheckOut - item.CheckIn).Days;
                if (nights > 0)
                {
                    total += nights * (item.Room?.PricePerNight ?? 0);
                }
            }

            return total;
        }



        public async Task<Cart> AddCartAsync(Cart cart)
        {
            var entry = await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return entry.Entity; 
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
        public async Task<Cart?> GetCartWithItemsAndRoomsByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Room)
                        .ThenInclude(r => r.Hotel) 
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task ClearCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                                           .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Room)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

    }
}

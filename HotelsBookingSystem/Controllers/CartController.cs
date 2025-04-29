using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Controllers
{
    [Authorize]
    public class CartController : Controller
    {

        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
        }
        public async Task<IActionResult> AddToCart(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (checkOut <= checkIn) return BadRequest("Invalid dates selected.");

            var cart = await _cartRepository.GetCartByUserIdAsync(user.Id);
            if (cart == null)
            {
                cart = new Cart { UserId = user.Id, CreatedAt = DateTime.Now };
                await _cartRepository.AddCartAsync(cart);
            }

            if (cart.CartItems == null)
                cart.CartItems = new List<CartItem>();

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.RoomId == roomId && ci.CheckIn == checkIn && ci.CheckOut == checkOut);
            if (existingItem == null)
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    RoomId = roomId,
                    CheckIn = checkIn,
                    CheckOut = checkOut
                };
                await _cartRepository.AddToCartAsync(cartItem);
            }

            await _cartRepository.SaveAsync();
            return RedirectToAction("Index", "Cart");
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(user.Id);

            return View(cart); 
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(id);
            if (cartItem != null)
            {
                await _cartRepository.DeleteCartItem(cartItem); 
                await _cartRepository.SaveAsync(); 
            }
            return Ok();
        }

    }

}


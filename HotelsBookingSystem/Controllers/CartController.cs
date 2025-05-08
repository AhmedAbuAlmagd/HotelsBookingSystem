using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelsBookingSystem.Controllers
{
    [Authorize]
    public class CartController : Controller
    {

        private readonly ICartRepository _cartRepository;
        HotelsContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly  IRoomRepository _roomRepository;

        public CartController(HotelsContext context, ICartRepository cartRepository, IRoomRepository roomRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _roomRepository = roomRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> AddToCart(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (checkOut <= checkIn || checkIn.Date < DateTime.Now.Date)
            {
                TempData["Error"] = "Invalid dates selected. Please ensure that the Check-out is after Check-in, and the Check-in is not in the past.";
                return RedirectToAction("Index", "Cart");
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(user.Id);
            if (cart == null)
            {
                cart = new Cart { UserId = user.Id, CreatedAt = DateTime.Now };
                cart = await _cartRepository.AddCartAsync(cart);
            }

            cart.CheckInDate = checkIn;
            cart.CheckOutDate = checkOut;

            if (cart.CartItems == null)
                cart.CartItems = new List<CartItem>();

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.RoomId == roomId);
            if (existingItem == null)
            {
                var roomPrice = _roomRepository.GetById(roomId)?.PricePerNight ?? 0;
                var nights = (checkOut - checkIn).Days;

                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    RoomId = roomId,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    TotalPrice = (decimal)(nights * roomPrice)
                };

                await _cartRepository.AddToCartAsync(cartItem);
            }

            await _cartRepository.SaveAsync();

            return RedirectToAction("Index", "Cart");
        }



        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var totalAmount = await _cartRepository.CalculateTotalAmountAsync(cart.Id);

            var cartViewModel = new CartViewModel
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                CreatedAt = (DateTime)cart.CreatedAt,
                TotalPrice = (int)totalAmount,
                CartItems = cart.CartItems.Select(item => new CartItemViewModel
                {
                    CartItemId = item.Id,
                    RoomId = item.RoomId,
                    RoomType = item.Room.Type,
                    CheckIn = item.CheckIn,
                    CheckOut = item.CheckOut,
                    Nights = (item.CheckOut - item.CheckIn).Days,
                    PricePerNight = item.Room.PricePerNight,
                    TotalPrice = item.Room.PricePerNight * (item.CheckOut - item.CheckIn).Days,
                    RoomImage = item.Room.RoomImages,
                    Room = item.Room
                }).ToList()
            };

            return View(cartViewModel);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _cartRepository.GetCartItemByIdAsync(id);
            if (item != null)
            {
                await _cartRepository.DeleteCartItem(item);
                await _cartRepository.SaveAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

    }

}


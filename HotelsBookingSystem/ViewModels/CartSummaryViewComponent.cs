using HotelsBookingSystem.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.ViewModels
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ICartRepository _cartRepository;

        public CartSummaryViewComponent(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            var count = cart?.CartItems?.Count ?? 0;

            return View(count);
        }
    }
}
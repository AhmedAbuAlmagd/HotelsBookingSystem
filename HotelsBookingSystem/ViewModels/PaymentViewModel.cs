using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class PaymentViewModel
    {

            internal string status;

            public int CartId { get; set; }
            public decimal TotalPrice { get; set; }
            public DateTime CheckIn { get; set; }
            public DateTime CheckOut { get; set; }
            public int GuestsCount { get; set; }
            public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        
    }
}

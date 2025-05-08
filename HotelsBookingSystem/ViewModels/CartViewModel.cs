using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public List<SelectedServiceViewModel> SelectedServices { get; set; } = new List<SelectedServiceViewModel>();
        public decimal TotalPrice { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public Room Room { get; set; }
        public string RoomImage { get; set; }
        public string ErrorMessage { get; set; }


        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public List<CartItem> Rooms { get; set; }
        public List<Service> AvailableServices { get; set; }
        public List<int> SelectedServiceIds { get; set; } = new List<int>();

    }
    }








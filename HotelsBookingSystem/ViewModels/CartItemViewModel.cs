using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class CartItemViewModel
    {
        public int RoomId { get; set; }
        public string RoomType { get; set; } 
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public int PricePerNight { get; set; }
        public Room Room { get; set; }
        public int cartId { get; set; }

            public int CartItemId { get; set; }  
            

        public int TotalPrice { get; set; }
        public List<RoomImage>? RoomImage { get; internal set; }
    }
}

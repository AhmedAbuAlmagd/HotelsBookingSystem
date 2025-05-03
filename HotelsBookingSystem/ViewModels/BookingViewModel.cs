namespace HotelsBookingSystem.ViewModels
{
    public class BookingViewModel
    {
        public string? Status { get; set; }
        public DateTime? Booking_date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? GuestsCount { get; set; }
        public int? paymentStatus { get; set; }

        public int TotalPrice { get; set; }
        public int HotelId { get; set; }
        public string UserId { get; set; }




    }
}

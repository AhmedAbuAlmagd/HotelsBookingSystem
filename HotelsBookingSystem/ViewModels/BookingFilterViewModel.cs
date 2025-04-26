namespace HotelsBookingSystem.ViewModels
{
    public class BookingFilterViewModel
    {
        public string Status { get; set; }
        public int? HotelId { get; set; }
        public DateTime? BookingDateFrom { get; set; }
        public DateTime? BookingDateTo { get; set; }
        public string ClientName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}

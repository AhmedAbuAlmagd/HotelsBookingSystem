namespace HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int TotalHotels { get; set; }
        public int TotalRooms { get; set; }
        public int ActiveClients { get; set; }
        public int TotalBookings { get; set; }
        public List<BookingViewModel> RecentBookings { get; set; }
        public List<HotelViewModel> Hotels { get; set; }
        public List<RoomViewModel> Rooms { get; set; }
        public List<ClientViewModel> Clients { get; set; }
    }
}

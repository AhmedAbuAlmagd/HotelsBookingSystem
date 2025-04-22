using HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard;

namespace HotelsBookingSystem.ViewModels.AdminViewModels
{
    public class HotelDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        //public int Rating { get; set; }   
        public string Status { get; set; }
        public int RoomCount { get; set; }
        public int AvailableRooms { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool ShowMap { get; set; } = true;
        public List<Dashboard.RoomViewModel> Rooms { get; set; } = new List<Dashboard.RoomViewModel>();
        public List<BookingViewModel> RecentBookings { get; set; } = new List<BookingViewModel>();
    }
}

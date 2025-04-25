using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels.AdminViewModels.HotelDetails
{
    public class HotelDetailsPageViewModel
    {
        public Hotel Hotel { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public PaginationInfo RoomPagination { get; set; }
        public PaginationInfo ServicePagination { get; set; }
    }
}

using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class HotelModelView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Phone { get; set; }
        public string? Status { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public List<string> HotelImages { get; set; }
        public List<Room> hotelRooms { get; set; }
        public List<string>roomimages { get; set; }
        public List<Service>? Services { get; set; }
    }
}

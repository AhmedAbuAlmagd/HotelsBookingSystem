namespace HotelsBookingSystem.Models
{
    public class Hotel_Service
    {
        public int HotelId {  get; set; }
        public int serviceId {  get; set; }

        public Hotel Hotel { get; set; }
        public Service Service { get; set; }
    }
}

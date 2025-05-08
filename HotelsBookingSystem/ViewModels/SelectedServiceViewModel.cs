using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class SelectedServiceViewModel
    {
        //public string ServiceName { get; set; }
        //public int Price { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int ServicePrice { get; set; }
        public Room Room { get; set; }
    }
}

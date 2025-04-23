namespace HotelsBookingSystem.ViewModels.AdminViewModels
{
    public class HotelsManagementViewModel
    {
        public List<HotelViewModel> Hotels { get; set; } = new List<HotelViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

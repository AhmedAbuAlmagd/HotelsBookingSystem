using HotelsBookingSystem.Models;
using HotelsBookingSystem.ViewModels.AdminViewModels;

namespace HotelsBookingSystem.Services
{
    public interface IHotelService
    {
        List<HotelViewModel> GetAllHotels();
        HotelViewModel MapToViewModel(Hotel hotel);
        HotelDetailsViewModel GetHotelDetails(int id);
        void AddHotel(HotelFormViewModel model);
        void UpdateHotel(int id, HotelFormViewModel model);
        void DeleteHotel(int id);
        int GetTotalHotelsCount(string searchTerm, string status, string city);
        List<HotelViewModel> GetHotelsPaged(int pageNumber, int pageSize ,  string  searchTerm , string  status , string city);
    }
}

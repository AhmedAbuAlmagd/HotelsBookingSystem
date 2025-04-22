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

    }
}

using System.Linq;
using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace HotelsBookingSystem.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelsContext con;

        public RoomRepository(HotelsContext context)
        {
            con = context;
        }
        IPagedList<Room> IRoomRepository.GetAll(int page, int pageSize)
        {
            var rooms = con.Rooms.Include(r => r.Hotel)
                .Include(r => r.RoomImages)
                .OrderBy(r => r.Id)   
                .ToPagedList(page, pageSize); ;
            return rooms;

        }

        Room IRoomRepository.GetById(int id)
        {

            //var room = con.Rooms.Find(id);
            var room = con.Rooms.Include(r => r.Hotel)
                .Include(r => r.RoomImages).FirstOrDefault(r => r.Id == id);
            if (room != null)
            {
                return room;
            }
            else
            {
                throw new Exception("Room not found");
            }
        }
        void IRoomRepository.Add(Room room)
        {

            con.Rooms.Add(room);
            
        }

        void IRoomRepository.Delete(int id)
        {
            var room = con.Rooms.Find(id);
            if (room != null)
            {
                con.Rooms.Remove(room);
            }
            else
            {
                throw new Exception("Room not found");
            }
        }

        void IRoomRepository.SaveChanges()
        {
           con.SaveChanges();
        }

        public void Update(Room room)
        {
           con.Rooms.Update(room);
        }
        public IEnumerable<Room> FilterRooms(string? type, int? minPrice, int? maxPrice, int? hotelId, string? city)
        {
            var query = con.Rooms
                .Include(r => r.RoomImages)
                .Include(r => r.Hotel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(type))
                query = query.Where(r => r.Type == type);

            if (minPrice.HasValue)
                query = query.Where(r => r.PricePerNight >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.PricePerNight <= maxPrice.Value);

            if (hotelId.HasValue)
                query = query.Where(r => r.HotelId == hotelId.Value);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(r => r.Hotel.City.Contains(city));

            return query.ToList();
        }

    }
}

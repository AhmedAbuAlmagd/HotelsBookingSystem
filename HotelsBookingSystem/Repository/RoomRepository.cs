﻿using System.Linq;
using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
 
using X.PagedList;
using X.PagedList.Extensions;

namespace HotelsBookingSystem.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelsContext con;

        public RoomRepository(HotelsContext context)
        {
            con = context;
        }
        public IPagedList<Room> GetAll(int page, int pageSize)
        {
            var rooms = con.Rooms
                .Where(r => r.Status == "available")   
                .Include(r => r.Hotel)                 
                .Include(r => r.RoomImages)           
                .OrderBy(r => r.Id)                  
                .ToPagedList(page, pageSize);        

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
        public IPagedList<Room> FilterRooms(string type, int? minPrice, int? maxPrice,
                                   int? hotelId, string city, int pageNumber, int pageSize)
        {
            var query = con.Rooms.Where(r=>r.Status == "available") 
                .Include(r => r.RoomImages)
                .Include(r => r.Hotel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(type))
                query = query.Where(r => r.Type.Contains(type));

            if (minPrice.HasValue)
                query = query.Where(r => r.PricePerNight >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.PricePerNight <= maxPrice.Value);

            if (hotelId.HasValue)
                query = query.Where(r => r.HotelId == hotelId.Value);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(r => r.Hotel.City.Contains(city));

            return query.OrderBy(r => r.Id).ToPagedList(pageNumber, pageSize);
        }


        public List<Hotel> GetAllhotels()
        {
           var hotels= con.Hotels.Where(h=>h.Status=="available")
                .Include(h => h.Rooms)
                .Include(h => h.HotelImages)
                .Include(h => h.Reviews)
                .Include(h => h.Bookings)
                .ToList();
            return hotels;
        }

        public List<Room> GetAllroom()
        {
            var rooms = con.Rooms
               .Where(r => r.Status == "available")
               .Include(r => r.Hotel)
               .Include(r => r.RoomImages)
              .ToList();


            return rooms;
        }
    }
}

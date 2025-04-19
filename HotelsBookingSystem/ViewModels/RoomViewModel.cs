﻿using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int PricePerNight { get; set; }
        public Hotel hotel { get; set; }
       public List<string> citys { get; set; }
        public List<string> RoomImages { get; set; }
        public List<Hotel> hotels { get; set; }
       

    }
}

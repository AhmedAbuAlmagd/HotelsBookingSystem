﻿using HotelsBookingSystem.Repository;
using HotelsBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IReviewRepository _ReviewRepository;
        private readonly IHotelRepository _HotelRepository;

        public AdminController(IAdminService adminService , IReviewRepository ReviewRepository , IHotelRepository hotelRepository)
        {
            _adminService = adminService;
            _ReviewRepository = ReviewRepository;
            _HotelRepository = hotelRepository;
        }

        public async Task<IActionResult> Dashboard()
        {
            var dashboardData = await _adminService.GetDashboardDataAsync();
            return View(dashboardData);
        }

        public IActionResult GetReviews(string hotelId = "", string rating = "", int page = 1)
        {
            ViewBag.CurrentHotelId = hotelId;
            ViewBag.CurrentRating = rating;

            ViewBag.Hotels = _HotelRepository.GetAllhotels().Where(x => x.Reviews.Count() > 0);

            int hotelIdValue = string.IsNullOrEmpty(hotelId) ? 0 : int.Parse(hotelId);
            int ratingValue = string.IsNullOrEmpty(rating) ? 0 : int.Parse(rating);

            var reviews = _ReviewRepository.GetAllReviews(hotelIdValue, ratingValue);

            int totalItems = reviews.Count();
            int pageSize = 10;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            reviews = reviews
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(reviews);
        }

    }
}

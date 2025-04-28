// ServiceController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HotelsBookingSystem.Services;
using HotelsBookingSystem.Models;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using HotelsBookingSystem.Repository;

namespace HotelsBookingSystem.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(
            IServiceRepository serviceRepository,
            ILogger<ServiceController> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetService(int id)
        {
            try
            {
                var service =  _serviceRepository.GetById(id);
                if (service == null)
                {
                    return NotFound(new { success = false, message = "Service not found" });
                }

                return Json(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service");
                return StatusCode(500, new { success = false, message = "An error occurred retrieving service details" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ServiceViewModelAd model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                    });
                }


                var service = new Service
                {
                    Name = model.Name,
                    Price = model.Price,
                    Description = model.Description
                };

                 _serviceRepository.Add(service);
                var hotelService = new Hotel_Service
                {
                    HotelId = model.HotelId,
                    serviceId = service.Id
                };
                _serviceRepository.AddHotelService(hotelService);


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service");
                return StatusCode(500, new { success = false, message = "An error occurred creating the service" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, ServiceViewModelAd model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                    });
                }

                var service = _serviceRepository.GetById(id);
                if (service == null)
                {
                    return NotFound(new { success = false, message = "Service not found" });
                }

                service.Name = model.Name;
                service.Price = model.Price;
                service.Description = model.Description;

                 _serviceRepository.Update(service);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service");
                return StatusCode(500, new { success = false, message = "An error occurred updating the service" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                 _serviceRepository.Delete(id);
                 return Json(new { success = true });
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting service: Service could not be deleted");
            }
        }
    }
}

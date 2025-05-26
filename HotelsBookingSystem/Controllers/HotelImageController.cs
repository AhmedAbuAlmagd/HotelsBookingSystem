using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels.HotelDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HotelsBookingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HotelImageController : Controller
    {
        private readonly HotelsContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HotelImageController(HotelsContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _context.HotelImages.FindAsync(id);
            if (image == null)
            {
                return Json(new { success = false, message = "Image not found" });
            }

            return Json(new
            {
                id = image.ImageId,
                hotelId = image.HotelId,
                imageUrl = image.ImageUrl,
                isPrimary = image.IsPrimary,
                caption = image.Caption
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelImageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid form data" });
            }

            try
            {
                var hotel = await _context.Hotels.FindAsync(model.HotelId);
                if (hotel == null)
                {
                    return Json(new { success = false, message = "Hotel not found" });
                }

                string uniqueFileName = null;
                if (model.Image != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/hotels");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "No image uploaded" });
                }

                var hotelImage = new HotelImage
                {
                    HotelId = model.HotelId,
                    ImageUrl = "/images/Hotels/" + uniqueFileName,
                    IsPrimary = false,
                    Caption = model.Caption
                };

                _context.HotelImages.Add(hotelImage);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error creating hotel image: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, HotelImageViewModel model)
        {
            if (id != model.Id)
            {
                return Json(new { success = false, message = "ID mismatch" });
            }

            ModelState.Remove("IsPrimary");
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid form data" });
            }

            try
            {
                var hotelImage = await _context.HotelImages.FindAsync(id);
                if (hotelImage == null)
                {
                    return Json(new { success = false, message = "Hotel image not found" });
                }

                if (model.Image != null)
                {
                    if (!string.IsNullOrEmpty(hotelImage.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, hotelImage.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/Hotels");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    hotelImage.ImageUrl = "/images/Hotels/" + uniqueFileName;
                }

              

                hotelImage.Caption = model.Caption;

                _context.Update(hotelImage);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating hotel image: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsPrimary(int id)
        {
            try
            {
                var hotelImage = await _context.HotelImages.FindAsync(id);
                if (hotelImage == null)
                {
                    return Json(new { success = false, message = "Hotel image not found" });
                }

                var primaryImages = await _context.HotelImages
                    .Where(i => i.HotelId == hotelImage.HotelId && i.IsPrimary)
                    .ToListAsync();

                foreach (var img in primaryImages)
                {
                    img.IsPrimary = false;
                }

                hotelImage.IsPrimary = true;

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error setting image as primary: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var hotelImage = await _context.HotelImages.FindAsync(id);
                if (hotelImage == null)
                {
                    return Json(new { success = false, message = "Hotel image not found" });
                }

                // Check if this is the only image or if it's primary
                bool isOnlyImage = await _context.HotelImages.CountAsync(i => i.HotelId == hotelImage.HotelId) == 1;

                if (isOnlyImage)
                {
                    return Json(new { success = false, message = "Cannot delete the only image of the hotel" });
                }

                // Delete image file from filesystem
                if (!string.IsNullOrEmpty(hotelImage.ImageUrl))
                {
                    string imagePath = Path.Combine(_hostEnvironment.WebRootPath, hotelImage.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // If deleting a primary image, set another one as primary
                if (hotelImage.IsPrimary)
                {
                    var nextImage = await _context.HotelImages
                        .Where(i => i.HotelId == hotelImage.HotelId && i.ImageId != hotelImage.ImageId)
                        .FirstOrDefaultAsync();

                    if (nextImage != null)
                    {
                        nextImage.IsPrimary = true;
                    }
                }

                _context.HotelImages.Remove(hotelImage);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting hotel image: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetImagesForHotel(int hotelId)
        {
            try
            {
                var images = await _context.HotelImages
                    .Where(i => i.HotelId == hotelId)
                    .OrderByDescending(i => i.IsPrimary)
                    .ThenBy(i => i.ImageId)
                    .Select(i => new
                    {
                        id = i.ImageId,
                        imageUrl = i.ImageUrl,
                        isPrimary = i.IsPrimary,
                        caption = i.Caption
                    })
                    .ToListAsync();

                return Json(new { success = true, images });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving images: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPrimaryImage(int hotelId)
        {
            try
            {
                var primaryImage = await _context.HotelImages
                    .FirstOrDefaultAsync(i => i.HotelId == hotelId && i.IsPrimary);

                if (primaryImage == null)
                {
                    // Fallback to any image if no primary exists
                    primaryImage = await _context.HotelImages
                        .FirstOrDefaultAsync(i => i.HotelId == hotelId);
                }

                if (primaryImage == null)
                {
                    return Json(new { success = false, message = "No images found for this hotel" });
                }

                return Json(new
                {
                    success = true,
                    imageUrl = primaryImage.ImageUrl,
                    caption = primaryImage.Caption
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving primary image: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCaption(int id, string caption)
        {
            try
            {
                var hotelImage = await _context.HotelImages.FindAsync(id);
                if (hotelImage == null)
                {
                    return Json(new { success = false, message = "Hotel image not found" });
                }

                hotelImage.Caption = caption;
                _context.Update(hotelImage);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating caption: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CountImages(int hotelId)
        {
            try
            {
                var count = await _context.HotelImages
                    .CountAsync(i => i.HotelId == hotelId);

                return Json(new { success = true, count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error counting images: " + ex.Message });
            }
        }

    }
}
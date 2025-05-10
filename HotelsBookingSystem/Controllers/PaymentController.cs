using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;

public class PaymentController : Controller
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICartRepository _cartRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PaymentController> _logger;
    private readonly IServiceRepository _serviceRepository;

    HotelsContext _context;

    public PaymentController(IBookingRepository bookingRepository, HotelsContext context, IPaymentRepository paymentRepository, ICartRepository cartRepository,
                             UserManager<ApplicationUser> userManager, ILogger<PaymentController> logger, IServiceRepository serviceRepository)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _cartRepository = cartRepository;
        _context = context;
        _logger = logger;
        _userManager = userManager;
        _serviceRepository = serviceRepository;
    }


    private BookingViewModel bookingViewModel;



    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);


        if (cart == null || !cart.CartItems.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        var checkIn = cart.CartItems.FirstOrDefault()?.CheckIn;
        var checkOut = cart.CartItems.FirstOrDefault()?.CheckOut;

        if (checkIn == null || checkOut == null)
        {
            TempData["Error"] = "Check-in or check-out dates are missing.";
            return RedirectToAction("Index", "Cart");
        }


        var totalPrice = cart.CartItems.Sum(ci => ci.Room.PricePerNight * ci.Nights);

        var model = new PaymentViewModel
        {
            TotalPrice = totalPrice,
            CheckIn = checkIn.Value,
            CheckOut = checkOut.Value
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(PaymentViewModel paymentViewModel)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null || !cart.CartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var checkIn = paymentViewModel.CheckIn;
        var checkOut = paymentViewModel.CheckOut;
        var nights = (checkOut - checkIn).Days;

        Dictionary<int, List<int>> servicesByHotel;
        try
        {
            servicesByHotel = string.IsNullOrEmpty(paymentViewModel.SelectedServiceIds)
                ? new Dictionary<int, List<int>>()
                : JsonSerializer.Deserialize<Dictionary<int, List<int>>>(paymentViewModel.SelectedServiceIds);
        }
        catch (JsonException ex)
        {
            TempData["Error"] = $"Error deserializing SelectedServiceIds: {ex.Message}";
            Console.WriteLine($"Error deserializing SelectedServiceIds: {ex.Message}");
            try
            {
                var serviceIds = JsonSerializer.Deserialize<List<int>>(paymentViewModel.SelectedServiceIds);
                servicesByHotel = new Dictionary<int, List<int>>();
                if (serviceIds != null && serviceIds.Any())
                {
                    var firstHotelId = cart.CartItems.FirstOrDefault()?.Room.HotelId ?? 0;
                    servicesByHotel[firstHotelId] = serviceIds;
                }
            }
            catch (Exception ex2)
            {
                TempData["Error"] = $"Fallback failed: {ex2.Message}";
                Console.WriteLine($"Fallback failed: {ex2.Message}");
                servicesByHotel = new Dictionary<int, List<int>>();
            }
        }

     
        var roomTotal = cart.CartItems.Sum(item => item.Room.PricePerNight * nights);
        decimal serviceTotal = 0;
        var allServices = new List<HotelsBookingSystem.Models.Service>();

        foreach (var hotelServices in servicesByHotel.Values)
        {
            var services = await _serviceRepository.GetServicesByIdsAsync(hotelServices);
            allServices.AddRange(services);
        }
        serviceTotal = allServices.Sum(s => s.Price * nights);

        var lineItems = cart.CartItems.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(item.Room.PricePerNight * 100),
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = $"Room {item.Room.Type}",
                    Description = $"{item.Room.Hotel?.Name} - {checkIn:yyyy-MM-dd} to {checkOut:yyyy-MM-dd}"
                }
            },
            Quantity = nights
        }).ToList();

        foreach (var servicee in allServices)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(servicee.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = servicee.Name,
                        Description = $"Service for {nights} night(s)"
                    }
                },
                Quantity = nights
            });
        }

        decimal calculatedTotal = roomTotal + serviceTotal;

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = $"{Request.Scheme}://{Request.Host}/Payment/PaymentSuccess?sessionId={{CHECKOUT_SESSION_ID}}",
            CancelUrl = Url.Action("PaymentCancel", "Payment", null, Request.Scheme),
            CustomerEmail = User.FindFirstValue(ClaimTypes.Email),
            Metadata = new Dictionary<string, string>
        {
            { "UserId", userId },
            { "CheckIn", checkIn.ToString("o") },
            { "CheckOut", checkOut.ToString("o") },
            { "TotalAmount", calculatedTotal.ToString() },
            { "SelectedServiceIds", JsonSerializer.Serialize(servicesByHotel) }
        }
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return Redirect(session.Url);
    }
    [HttpGet]
    public async Task<IActionResult> PaymentSuccess(string sessionId)
    {
        var sessionService = new SessionService();
        var session = await sessionService.GetAsync(sessionId);

        if (session.PaymentStatus != "paid")
        {
            TempData["Error"] = "Payment not completed";
            return RedirectToAction("Index", "Cart");
        }

        var userId = session.Metadata["UserId"];
        var checkIn = DateTime.Parse(session.Metadata["CheckIn"]);
        var checkOut = DateTime.Parse(session.Metadata["CheckOut"]);
        var totalAmount = decimal.Parse(session.Metadata["TotalAmount"]);
        var servicesByHotelJson = session.Metadata["SelectedServiceIds"];
        Dictionary<int, List<int>> servicesByHotel;
        try
        {
            servicesByHotel = string.IsNullOrEmpty(servicesByHotelJson)
                ? new Dictionary<int, List<int>>()
                : JsonSerializer.Deserialize<Dictionary<int, List<int>>>(servicesByHotelJson);
        }
        catch (JsonException ex)
        {

            try
            {
                var serviceIds = JsonSerializer.Deserialize<List<int>>(servicesByHotelJson);
                servicesByHotel = new Dictionary<int, List<int>>();
                if (serviceIds != null && serviceIds.Any())
                {
                    var firstHotelId = (await _cartRepository.GetCartByUserIdAsync(userId))
                        ?.CartItems.FirstOrDefault()?.Room.HotelId ?? 0;
                    servicesByHotel[firstHotelId] = serviceIds;
                }
            }
            catch (Exception ex2)
            {
                servicesByHotel = new Dictionary<int, List<int>>();
            }
        }

      
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null || !cart.CartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var nights = (checkOut - checkIn).Days;

        var roomsByHotel = cart.CartItems.GroupBy(item => item.Room.HotelId);

        List<int> bookingIds = new List<int>();
        Payment lastPayment = null;

        foreach (var hotelGroup in roomsByHotel)
        {
            var hotelId = hotelGroup.Key;
            var hotelRooms = hotelGroup.ToList();
            decimal hotelRoomTotal = hotelRooms.Sum(item => item.Room.PricePerNight * nights);
            decimal hotelServiceTotal = 0;


            var booking = new Booking
            {
                UserId = userId,
                Status = "Confirmed",
                Booking_date = DateTime.Now,
                HotelId = (int)hotelId,
                TotalPrice = (int)(hotelRoomTotal + hotelServiceTotal),
                CheckIn = checkIn,
                CheckOut = checkOut,
                GuestsCount = hotelRooms.Count
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            bookingIds.Add(booking.Id);

            Console.WriteLine($"Created Booking ID: {booking.Id} for Hotel ID: {hotelId}");

            foreach (var item in hotelRooms)
            {
                var bookingRoom = new BookingRoom
                {
                    RoomId = item.RoomId,
                    BookingId = booking.Id
                };
                _context.BookingRooms.Add(bookingRoom);
            }
            await _context.SaveChangesAsync();

            if (servicesByHotel.ContainsKey((int)hotelId) && servicesByHotel[(int)hotelId].Any())
            {
                var serviceIds = servicesByHotel[(int)hotelId].Distinct().ToList();
                var services = await _serviceRepository.GetServicesByIdsAsync(serviceIds);
                hotelServiceTotal = services.Sum(s => s.Price * nights);

               

                var bookingServices = new List<BookingService>();
                foreach (var service in services)
                {
                    var bookingService = new BookingService
                    {
                        BookingID = booking.Id,
                        ServiceID = service.Id,
                    };
                    bookingServices.Add(bookingService);
                }

                try
                {
                    _context.BookingServices.AddRange(bookingServices);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    
                    return RedirectToAction("Index", "Cart");
                }

                booking.TotalPrice = (int)(hotelRoomTotal + hotelServiceTotal);
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
            }

            var payment = new Payment
            {
                BookingID = booking.Id,
                Amount = booking.TotalPrice,
                status = "Completed",
                Method = "Stripe",
                Date = DateTime.Now,
                TransactionId = $"{session.Id}_{booking.Id}"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            lastPayment = payment;
        }

        await _cartRepository.ClearCartByUserIdAsync(userId);

        return View("Success", new PaymentSuccessViewModel
        {
            BookingId = bookingIds.First(),
            TransactionId = lastPayment?.TransactionId,
            AmountPaid = lastPayment?.Amount ?? 0
        });
    }
}
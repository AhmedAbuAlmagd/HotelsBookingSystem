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

public class PaymentController : Controller
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICartRepository _cartRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PaymentController> _logger;
    HotelsContext _context;

    public PaymentController(IBookingRepository bookingRepository, HotelsContext context, IPaymentRepository paymentRepository, ICartRepository cartRepository,
                             UserManager<ApplicationUser> userManager, ILogger<PaymentController> logger)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _cartRepository = cartRepository;
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }


    private BookingViewModel bookingViewModel;


    [Authorize]
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
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }


        var checkIn = paymentViewModel.CheckIn;
        var checkOut = paymentViewModel.CheckOut;
        var nights = (checkOut - checkIn).Days;

        var serviceTotal = paymentViewModel.TotalPrice - cart.CartItems.Sum(c => c.Room.PricePerNight * nights);
        var servicePerNight = serviceTotal > 0 ? serviceTotal / nights : 0;

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

        if (serviceTotal > 0)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(servicePerNight * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Additional Services",
                        Description = "Selected hotel services"
                    }
                },
                Quantity = nights
            });
        }

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
            { "TotalAmount", paymentViewModel.TotalPrice.ToString() }
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

        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null || !cart.CartItems.Any())
        {
            TempData["Error"] = "Cart items not found";
            return RedirectToAction("Index", "Cart");
        }

        //var booking = new Booking
        //{
        //    UserId = userId,
        //    Status = "Confirmed",
        //    Booking_date = DateTime.Now,
        //    HotelId = 1,
        //    TotalPrice = (int)(session.AmountTotal / 100),
        //    CheckIn = checkIn,
        //    CheckOut = checkOut,
        //    GuestsCount = cart.CartItems.Count
        //};

        //_context.Bookings.Add(booking);
        //await _context.SaveChangesAsync();

        //foreach (var item in cart.CartItems)
        //{
        //    var bookingRoom = new BookingRoom
        //    {
        //        RoomId = item.RoomId,
        //        BookingId = booking.Id
        //    };
        //    _context.BookingRooms.Add(bookingRoom);

        //}
        var booking = new Booking
        {
            UserId = userId,
            Status = "Confirmed",
            Booking_date = DateTime.Now,
            HotelId = 1,
            TotalPrice = (int)(session.AmountTotal / 100),
            CheckIn = cart.CartItems.First().CheckIn,
            CheckOut = cart.CartItems.First().CheckOut,
            GuestsCount = cart.CartItems.Count
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        foreach (var item in cart.CartItems)
        {
            var bookingRoom = new BookingRoom
            {
                RoomId = item.RoomId,
                BookingId = booking.Id
            };
            _context.BookingRooms.Add(bookingRoom);
        }


        var payment = new Payment
        {
            BookingID = booking.Id,
            Amount = booking.TotalPrice,
            status = "Completed",
            Method = "Stripe",
            Date = DateTime.Now,
            TransactionId = session.PaymentIntentId
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        await _cartRepository.ClearCartByUserIdAsync(userId);

        return View("Success", new PaymentSuccessViewModel
        {
            BookingId = booking.Id,
            TransactionId = payment.TransactionId,
            AmountPaid = payment.Amount
        });
    }

    public IActionResult Success()
    {
        return View();
    }

}





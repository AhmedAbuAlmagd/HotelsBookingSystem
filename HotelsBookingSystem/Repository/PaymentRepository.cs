using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;

namespace HotelsBookingSystem.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly HotelsContext _context;

        public PaymentRepository(HotelsContext context)
        {
            _context = context;
        }

        public void AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

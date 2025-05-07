using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface IPaymentRepository
    {
            void AddPayment(Payment payment);
            Task SaveAsync();

    }
}

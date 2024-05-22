using CinemaWebApplication.Models;

namespace CinemaWebApplication.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ProcessPayment(int userId, int screeningId, double amount, int ticketCount)
        {
            var payment = new Payment
            {
                TicketId = screeningId,
                UserId = userId,
                PaymentDate = DateTime.Now,
                Amount = amount,
                TicketCount = ticketCount
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

        }
    }

}

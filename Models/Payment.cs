using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaWebApplication.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public int TicketCount { get; set; }

    }
}

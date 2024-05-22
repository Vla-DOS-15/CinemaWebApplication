namespace CinemaWebApplication.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ScreeningId { get; set; }
        public virtual Screening Screening { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime PurchaseDate { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

    }
}

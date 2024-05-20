using System.Net.Sockets;

namespace CinemaWebApplication.Models
{
    public class Screening
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
        public DateTime ScreeningTime { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }
    }
}

namespace CinemaWebApplication.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
        public int TotalTicketsSold { get; set; }
        public double TotalRevenue { get; set; }
    }

}

namespace CinemaWebApplication.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int ScreeningId { get; set; }
        public virtual Screening Screening { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }
        public bool IsAvailable { get; set; }
    }

}

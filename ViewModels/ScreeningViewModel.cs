using CinemaWebApplication.Models;

namespace CinemaWebApplication.ViewModels
{
    public class ScreeningViewModel
    {
        public Screening Screening { get; set; }
        public List<Seat> Seats { get; set; }
    }

}

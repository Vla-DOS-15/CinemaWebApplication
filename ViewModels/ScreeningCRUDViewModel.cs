using System.ComponentModel.DataAnnotations;

namespace CinemaWebApplication.ViewModels
{
    public class ScreeningCRUDViewModel
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public DateTime ScreeningTime { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid price")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid number of rows")]
        public int Rows { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid number of seats per row")]
        public int SeatsPerRow { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CinemaWebApplication.ViewModels
{
    public class PaymentViewModel
    {
        public int ScreeningId { get; set; }
        public string MovieTitle { get; set; }
        public int TicketCount { get; set; }

        [Display(Name = "Загальна сума")]
        public double Amount { get; set; }

        [Required]
        [Display(Name = "Номер картки")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "CVV")]
        public string CVV { get; set; }

        [Required]
        [Display(Name = "Термін дії")]
        public string ExpiryDate { get; set; }
    }
}

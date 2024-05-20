namespace CinemaWebApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<PersonalizedRecommendation> PersonalizedRecommendations { get; set; }
    }
}

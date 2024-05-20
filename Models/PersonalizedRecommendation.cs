namespace CinemaWebApplication.Models
{
    public class PersonalizedRecommendation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
        public DateTime RecommendationDate { get; set; }
    }
}

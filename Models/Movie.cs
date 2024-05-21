namespace CinemaWebApplication.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }
        public float Rating { get; set; }
        public string ImageUrl { get; set; }
        public virtual ICollection<MovieActor> MovieActors { get; set; }
        public virtual ICollection<Screening> Screenings { get; set; }
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }

    }
}

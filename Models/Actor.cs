namespace CinemaWebApplication.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
        public string Bio { get; set; }
        public virtual ICollection<MovieActor> MovieActors { get; set; }
    }
}

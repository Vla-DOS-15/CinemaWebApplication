using CinemaWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApplication
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Movies.Any())
                {
                    return; // База даних вже ініціалізована
                }
               

                // Додавання акторів
                var actor1 = new Actor
                {
                    Name = "Sample Actor 1",
                    Birthdate = new DateTime(1980, 1, 1),
                    Bio = "Sample Bio 1",
                    ImageUrl = "img"
                };
                var actor2 = new Actor
                {
                    Name = "Sample Actor 2",
                    Birthdate = new DateTime(1990, 2, 2),
                    Bio = "Sample Bio 2",
                    ImageUrl = "img"
                };
                context.Actors.AddRange(actor1, actor2);
                context.SaveChanges();

                // Додавання жанрів
                var genre1 = new Genre
                {
                    Name = "Action"
                };
                var genre2 = new Genre
                {
                    Name = "Drama"
                };
                var genre3 = new Genre
                {
                    Name = "Fantasy"
                };
                context.Genres.AddRange(genre1, genre2, genre3);
                context.SaveChanges();

                // Додавання фільмів
                var movie1 = new Movie
                {
                    Title = "Sample Movie 1",
                    Description = "Sample Description 1",
                    ReleaseDate = DateTime.Now.AddMonths(-1),
                    Duration = 120,
                    Rating = 4.5f,
                    ImageUrl = "https://lux.fm/uploads/media_news/2023/07/64c7bf453921d854780893.jpg?w=400&fit=cover&output=webp&q=85",
                    TrailerUrl = "https://youtu.be/4pcusbhhfQA",
                    MovieActors = new List<MovieActor>(),
                    MovieGenres = new List<MovieGenre>(),
                    Screenings = new List<Screening>()
                };
                var movie2 = new Movie
                {
                    Title = "Sample Movie 2",
                    Description = "Sample Description 2",
                    ReleaseDate = DateTime.Now.AddMonths(-2),
                    Duration = 150,
                    Rating = 4.0f,
                    ImageUrl = "https://static.hdrezka.ac/i/2023/6/14/l31bdcc5c6c24ig40c87l.png",
                    TrailerUrl = "https://youtu.be/4pcusbhhfQA",
                    MovieActors = new List<MovieActor>(),
                    MovieGenres = new List<MovieGenre>(),
                    Screenings = new List<Screening>()
                };
                context.Movies.AddRange(movie1, movie2);
                context.SaveChanges();

                // Додавання зв'язків між фільмами та акторами
                var movieActor1 = new MovieActor
                {
                    MovieId = movie1.Id,
                    ActorId = actor1.Id
                };
                var movieActor2 = new MovieActor
                {
                    MovieId = movie1.Id,
                    ActorId = actor2.Id
                };
                var movieActor3 = new MovieActor
                {
                    MovieId = movie2.Id,
                    ActorId = actor1.Id
                };
                context.MovieActors.AddRange(movieActor1, movieActor2, movieActor3);
                context.SaveChanges();

                // Додавання зв'язків між фільмами та жанрами
                var movieGenre1 = new MovieGenre
                {
                    MovieId = movie1.Id,
                    GenreId = genre1.Id
                };
                var movieGenre2 = new MovieGenre
                {
                    MovieId = movie1.Id,
                    GenreId = genre2.Id
                };
                var movieGenre3 = new MovieGenre
                {
                    MovieId = movie2.Id,
                    GenreId = genre1.Id
                };
                var movieGenre4 = new MovieGenre
                {
                    MovieId = movie2.Id,
                    GenreId = genre3.Id
                };
                context.MovieGenres.AddRange(movieGenre1, movieGenre2, movieGenre3, movieGenre4);
                context.SaveChanges();

                // Додавання сеансів
                var screening1 = new Screening
                {
                    MovieId = movie1.Id,
                    ScreeningTime = DateTime.Now.AddHours(1),
                    Seats = new List<Seat>()
                };
                var screening2 = new Screening
                {
                    MovieId = movie2.Id,
                    ScreeningTime = DateTime.Now.AddHours(2),
                    Seats = new List<Seat>()
                };
                context.Screenings.AddRange(screening1, screening2);
                context.SaveChanges();

                // Додавання місць
                for (int row = 1; row <= 10; row++)
                {
                    for (int number = 1; number <= 10; number++)
                    {
                        var seat1 = new Seat
                        {
                            ScreeningId = screening1.Id,
                            Row = row,
                            Number = number,
                            IsAvailable = true
                        };
                        var seat2 = new Seat
                        {
                            ScreeningId = screening2.Id,
                            Row = row,
                            Number = number,
                            IsAvailable = true
                        };
                        context.Seats.AddRange(seat1, seat2);
                    }
                }
                context.SaveChanges();

                context.Users.Add(
                    new User {
                        Username = "Admin",
                        Email = "admin@gmail.com",
                        Password = "1234",
                        RegistrationDate = DateTime.Now,
                        RoleName = "Admin"
                    });
                context.SaveChanges();
            }
        }
    }

}

using CinemaWebApplication.Interfaces;
using CinemaWebApplication.Models;
using CinemaWebApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWebApplication.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly ApplicationDbContext _context;

        public MoviesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> Details(int? id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return null;
            }

            return movie;
        }

        public async Task Create(MovieViewModel model)
        {
            var movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                ReleaseDate = model.ReleaseDate,
                Duration = model.Duration,
                Rating = model.Rating,
                ImageUrl = model.ImageUrl,
                TrailerUrl = model.TrailerUrl
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            // Додавання зв'язків з акторами та жанрами
            if (model.SelectedActors != null)
            {
                foreach (var actorId in model.SelectedActors)
                {
                    _context.MovieActors.Add(new MovieActor { MovieId = movie.Id, ActorId = actorId });
                }
            }

            if (model.SelectedGenres != null)
            {
                foreach (var genreId in model.SelectedGenres)
                {
                    _context.MovieGenres.Add(new MovieGenre { MovieId = movie.Id, GenreId = genreId });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task Edit(int id, MovieViewModel model)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.MovieGenres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return;
            }

            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.ReleaseDate = model.ReleaseDate;
            movie.Duration = model.Duration;
            movie.Rating = model.Rating;
            movie.ImageUrl = model.ImageUrl;
            movie.TrailerUrl = model.TrailerUrl;

            var existingActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();
            var newActorIds = model.SelectedActors ?? new List<int>();

            var actorsToAdd = newActorIds.Except(existingActorIds).ToList();
            var actorsToRemove = existingActorIds.Except(newActorIds).ToList();

            foreach (var actorId in actorsToAdd)
            {
                _context.MovieActors.Add(new MovieActor { MovieId = movie.Id, ActorId = actorId });
            }

            foreach (var actorId in actorsToRemove)
            {
                var actorToRemove = movie.MovieActors.FirstOrDefault(ma => ma.ActorId == actorId);
                if (actorToRemove != null)
                {
                    _context.MovieActors.Remove(actorToRemove);
                }
            }

            var existingGenreIds = movie.MovieGenres.Select(mg => mg.GenreId).ToList();
            var newGenreIds = model.SelectedGenres ?? new List<int>();

            var genresToAdd = newGenreIds.Except(existingGenreIds).ToList();
            var genresToRemove = existingGenreIds.Except(newGenreIds).ToList();

            foreach (var genreId in genresToAdd)
            {
                _context.MovieGenres.Add(new MovieGenre { MovieId = movie.Id, GenreId = genreId });
            }

            foreach (var genreId in genresToRemove)
            {
                var genreToRemove = movie.MovieGenres.FirstOrDefault(mg => mg.GenreId == genreId);
                if (genreToRemove != null)
                {
                    _context.MovieGenres.Remove(genreToRemove);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                    .ThenInclude(a=>a.Actor)
                .Include(m => m.MovieGenres)
                    .ThenInclude(a => a.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return;
            }

            // Видалення зв'язаних записів
            _context.MovieActors.RemoveRange(movie.MovieActors);
            _context.MovieGenres.RemoveRange(movie.MovieGenres);

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
    }
}

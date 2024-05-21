using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CinemaWebApplication.ViewModels;
using CinemaWebApplication.Models; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CinemaWebApplication.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CinemaWebApplication.Controllers
{
    public class AdminMoviesController : Controller
    {
        private readonly IScreeningService _screeningService;
        private readonly ApplicationDbContext _context;


        public AdminMoviesController(IScreeningService screeningService, ApplicationDbContext context)
        {
            _context = context;
            _screeningService = screeningService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }


        // GET: AdminMovies/Create
        public IActionResult Create()
        {
            var model = new MovieViewModel
            {
                Actors = _context.Actors.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList(),
                Genres = _context.Genres.Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                }).ToList()
            };
            return View(model);
        }

        // POST: AdminMovies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            
                var movie = new Movie
                {
                    Title = model.Title,
                    Description = model.Description,
                    ReleaseDate = model.ReleaseDate,
                    Duration = model.Duration,
                    Rating = model.Rating,
                    ImageUrl = model.ImageUrl
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
                return RedirectToAction(nameof(Index));
            
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.MovieGenres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var model = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Duration = movie.Duration,
                Rating = movie.Rating,
                ImageUrl = movie.ImageUrl,
                SelectedActors = movie.MovieActors.Select(ma => ma.ActorId).ToList(),
                SelectedGenres = movie.MovieGenres.Select(mg => mg.GenreId).ToList(),
                Actors = await _context.Actors.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToListAsync(),
                Genres = await _context.Genres.Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            
            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.MovieGenres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.ReleaseDate = model.ReleaseDate;
            movie.Duration = model.Duration;
            movie.Rating = model.Rating;
            movie.ImageUrl = model.ImageUrl;

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

            return RedirectToAction(nameof(Index));
            
        }



        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
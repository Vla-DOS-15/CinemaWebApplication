using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CinemaWebApplication.ViewModels;
using CinemaWebApplication.Models; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CinemaWebApplication.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using CinemaWebApplication.Services;


namespace CinemaWebApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminMoviesController : Controller
    {
        private readonly IScreeningService _screeningService;
        private readonly IMoviesService _moviesService;
        private readonly ApplicationDbContext _context;


        public AdminMoviesController(IScreeningService screeningService, ApplicationDbContext context, IMoviesService moviesService)
        {
            _context = context;
            _screeningService = screeningService;
            _moviesService = moviesService;
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
            var movie = _moviesService.Details(id);
            
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
            if(model != null)
                _moviesService.Create(model);

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
                TrailerUrl = movie.TrailerUrl,
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
                return NotFound();
            
            var movie = _moviesService.Edit(id, model);

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
            _moviesService.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
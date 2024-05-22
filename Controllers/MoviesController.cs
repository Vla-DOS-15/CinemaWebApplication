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
    public class MoviesController : Controller
    {
        private readonly IScreeningService _screeningService;
        private readonly ApplicationDbContext _context;


        public MoviesController(IScreeningService screeningService, ApplicationDbContext context)
        {
            _context = context;
            _screeningService = screeningService;
        }

        public async Task<IActionResult> Index(string searchString, string genre, DateTime? startDate, DateTime? endDate)
        {
            var movies = await _screeningService.GetUpcomingScreeningsAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                movies = movies.Where(m => m.MovieGenres.Any(mg => mg.Genre.Name == genre));
            }

            if (startDate.HasValue)
            {
                movies = movies.Where(m => m.Screenings.Any(s => s.ScreeningTime >= startDate));
            }

            if (endDate.HasValue)
            {
                movies = movies.Where(m => m.Screenings.Any(s => s.ScreeningTime <= endDate));
            }

            var genreList = await _context.Genres.ToListAsync();
            ViewBag.Genres = new SelectList(genreList, "Name", "Name");

            ViewData["CurrentFilter"] = searchString;
            ViewData["SelectedGenre"] = genre;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _screeningService.GetMovieByIdAsync(id);


            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        public async Task<IActionResult> PersonalizedRecommendations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);



            var recommendedMovies = await _context.PersonalizedRecommendations
                .Where(r => r.UserId == _context.Users.FirstOrDefault(u=>u.Email == User.Identity.Name).Id)
                .Include(r => r.Movie)
                    .ThenInclude(m => m.MovieGenres)
                        .ThenInclude(mg => mg.Genre)
                .Include(r => r.Movie)
                    .ThenInclude(m => m.Screenings)
                .Select(r => r.Movie)
                .ToListAsync();

            return View(recommendedMovies);
        }

    }

}
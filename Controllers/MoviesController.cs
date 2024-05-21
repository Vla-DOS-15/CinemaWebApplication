using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CinemaWebApplication.ViewModels;
using CinemaWebApplication.Models; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CinemaWebApplication.Interfaces;


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

        public async Task<IActionResult> Index()
        {
            var movies = await _screeningService.GetUpcomingScreeningsAsync();
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


        
    }

}
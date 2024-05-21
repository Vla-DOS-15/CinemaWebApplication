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
    public class ScreeningsController : Controller
    {
        private readonly IScreeningService _screeningService;

        public ScreeningsController(IScreeningService screeningService)
        {
            _screeningService = screeningService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _screeningService.GetUpcomingScreeningsAsync();
            return View(movies);
        }
    }

}
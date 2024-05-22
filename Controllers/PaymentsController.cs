using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CinemaWebApplication.Models;
using CinemaWebApplication.ViewModels;
using CinemaWebApplication.Services;

namespace CinemaWebApplication.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PaymentService _paymentService;

        public PaymentsController(ApplicationDbContext context, PaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        [HttpGet]
        public IActionResult Create(int screeningId, int ticketCount, double amount)
        {
            var screening = _context.Screenings.Include(s => s.Movie).FirstOrDefault(s => s.Id == screeningId);
            if (screening == null)
            {
                return NotFound();
            }

            var model = new PaymentViewModel
            {
                ScreeningId = screeningId,
                MovieTitle = screening.Movie.Title,
                TicketCount = ticketCount,
                Amount = amount
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ticket = await _context.Tickets
                    .Include(x => x.Screening)
                    .ThenInclude(t => t.Movie)
                    .ThenInclude(m => m.MovieGenres)
                    .FirstOrDefaultAsync(x => x.Screening.Id == model.ScreeningId);

                var userId = GetUserId();
                await _paymentService.ProcessPayment(userId, model.ScreeningId, model.Amount, model.TicketCount);

                var movieGenres = ticket.Screening.Movie.MovieGenres.Select(mg => mg.GenreId).ToList();
                var recommendedMovies = await _context.Movies
                    .Include(m => m.MovieGenres)
                    .Where(m => m.MovieGenres.Any(mg => movieGenres.Contains(mg.GenreId)))
                    .ToListAsync();

                foreach (var movie in recommendedMovies)
                {
                    bool alreadyRecommended = await _context.PersonalizedRecommendations
                        .AnyAsync(r => r.MovieId == movie.Id && r.UserId == userId);

                    if (!alreadyRecommended)
                    {
                        _context.PersonalizedRecommendations.Add(new PersonalizedRecommendation
                        {
                            MovieId = movie.Id,
                            UserId = userId,
                            RecommendationDate = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Movies");
            }

            return View(model);
        }




        private int GetUserId() => _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name).Id;
    }
}

using CinemaWebApplication.Models;
using CinemaWebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CinemaWebApplication.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Перегляд доступних фільмів
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }

        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> BookSeats(int id)
        {
            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (screening == null)
            {
                return NotFound();
            }

            var model = new ScreeningViewModel
            {
                Screening = screening,
                Seats = screening.Seats.ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public async Task<IActionResult> BookSeats(int screeningId, int[] selectedSeats)
        {
            

            if (selectedSeats == null || selectedSeats.Length == 0)
            {
                return BadRequest("No seats selected.");
            }

            var seats = await _context.Seats
                .Where(s => selectedSeats.Contains(s.Id) && s.IsAvailable)
                .ToListAsync();

            if (seats.Count != selectedSeats.Length)
            {
                return BadRequest("One or more seats are not available.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                return BadRequest("Unauthorized");
            }

            var screening = await _context.Screenings.FirstOrDefaultAsync(s => s.Id == screeningId);
            decimal totalPrice = screening.Price * seats.Count;

            foreach (var seat in seats)
            {
                seat.IsAvailable = false;

                _context.Tickets.Add(new Ticket
                {
                    ScreeningId = screeningId,
                    UserId = user.Id,
                    PurchaseDate = DateTime.Now
                });
            }

            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.MovieId == screening.MovieId);
            if (sale == null)
            {
                sale = new Sale
                {
                    MovieId = screening.MovieId,
                    TotalTicketsSold = 0,
                    TotalRevenue = 0
                };
                _context.Sales.Add(sale);
            }

            sale.TotalTicketsSold += seats.Count;
            sale.TotalRevenue += totalPrice;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Movies", new { id = screening.MovieId });
        }


        // Перегляд деталей фільму та розкладу сеансів
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Screenings)
                .Include(m => m.MovieActors)
                .ThenInclude(m=>m.Actor)
                .Include(m => m.MovieGenres)
                .ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        public async Task<IActionResult> SelectSeats(int id)
        {
            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (screening == null)
            {
                return NotFound();
            }
            return View(screening);
        }

        // Підтвердження покупки
        [HttpPost]
        public async Task<IActionResult> Purchase(int id)
        {
            List<int> selectedSeats = new List<int> ();
            selectedSeats.Add(34);
            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (screening == null || selectedSeats == null || !selectedSeats.Any())
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            foreach (var seatId in selectedSeats)
            {
                var seat = await _context.Seats.FirstOrDefaultAsync(se => se.Id == seatId);
                if (seat != null && seat.IsAvailable)
                {
                    seat.IsAvailable = false;
                    var ticket = new Ticket
                    {
                        ScreeningId = id,
                        UserId = int.Parse(userId),
                        PurchaseDate = DateTime.Now
                    };
                    _context.Tickets.Add(ticket);
                }
            }

            await _context.SaveChangesAsync();

            // Перехід до сторінки оплати
            return RedirectToAction("Payment", new { id, selectedSeats });
        }


        // Оплата
        public IActionResult Payment(int screeningId, List<int> selectedSeats)
        {
            // Логіка для відображення сторінки оплати
            return View();
        }

        // Підтвердження оплати
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int screeningId, List<int> selectedSeats)
        {
            // Логіка для підтвердження оплати

            // Після успішної оплати:
            // - Генерація квитків
            // - Надсилання підтвердження на електронну пошту
            // - Збереження квитків у базі даних

            return RedirectToAction("Confirmation");
        }

        // Підтвердження покупки
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaWebApplication.Models;
using CinemaWebApplication.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWebApplication.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var statistics = await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.Screenings)
                    .ThenInclude(s => s.Tickets)
                        .ThenInclude(t => t.Payments)
                .Select(m => new PaymentStatisticsViewModel
                {
                    MovieTitle = m.Title,
                    TotalTicketsSold = m.Screenings.SelectMany(s => s.Tickets)
                                                   .SelectMany(t => t.Payments)
                                                   .Sum(p => p.TicketCount),
                    TotalRevenue = m.Screenings.SelectMany(s => s.Tickets)
                                               .SelectMany(t => t.Payments)
                                               .Sum(p => (decimal)p.Amount)
                })
                .ToListAsync();

            return View(statistics);
        }
    }
}

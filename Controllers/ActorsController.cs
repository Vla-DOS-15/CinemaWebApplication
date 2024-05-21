using CinemaWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaWebApplication.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await _context.Actors
                .Include(a => a.MovieActors)
                    .ThenInclude(ma => ma.Movie)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }
    }
}

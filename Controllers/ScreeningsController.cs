using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaWebApplication.Models;
using CinemaWebApplication.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CinemaWebApplication.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ScreeningsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScreeningsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Screenings
        public async Task<IActionResult> Index()
        {
            var screenings = await _context.Screenings.Include(s => s.Movie).ToListAsync();
            return View(screenings);
        }

        // GET: Screenings/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
            return View();
        }

        // POST: Screenings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScreeningCRUDViewModel model)
        {
            if (ModelState.IsValid)
            {
                var screening = new Screening
                {
                    MovieId = model.MovieId,
                    ScreeningTime = model.ScreeningTime,
                    Price = model.Price
                };

                _context.Screenings.Add(screening);
                await _context.SaveChangesAsync();

                // Додавання місць для сеансу
                for (int row = 1; row <= model.Rows; row++)
                {
                    for (int number = 1; number <= model.SeatsPerRow; number++)
                    {
                        _context.Seats.Add(new Seat
                        {
                            ScreeningId = screening.Id,
                            Row = row,
                            Number = number,
                            IsAvailable = true
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", model.MovieId);
            return View(model);
        }

        // GET: Screenings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var screening = await _context.Screenings
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (screening == null)
            {
                return NotFound();
            }

            var model = new ScreeningCRUDViewModel
            {
                Id = screening.Id,
                MovieId = screening.MovieId,
                ScreeningTime = screening.ScreeningTime,
                Price = screening.Price,
                Rows = screening.Seats.Any() ? screening.Seats.Max(s => s.Row) : 0,
                SeatsPerRow = screening.Seats.Any() ? screening.Seats.Max(s => s.Number) : 0
            };

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", screening.MovieId);
            return View(model);
        }

        // POST: Screenings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ScreeningCRUDViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var screening = await _context.Screenings
                        .Include(s => s.Seats)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    screening.MovieId = model.MovieId;
                    screening.ScreeningTime = model.ScreeningTime;
                    screening.Price = model.Price;

                    _context.Update(screening);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScreeningExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", model.MovieId);
            return View(model);
        }

        // GET: Screenings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (screening == null)
            {
                return NotFound();
            }

            return View(screening);
        }

        // POST: Screenings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var screening = await _context.Screenings.FindAsync(id);
            _context.Screenings.Remove(screening);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScreeningExists(int id)
        {
            return _context.Screenings.Any(e => e.Id == id);
        }
    }
}

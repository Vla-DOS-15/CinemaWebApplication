using CinemaWebApplication.Interfaces;
using CinemaWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApplication.Services
{
    public class ScreeningService : IScreeningService
    {
        private readonly ApplicationDbContext _context;

        public ScreeningService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetUpcomingScreeningsAsync()
        {
            var currentDate = DateTime.Now.Date;
            return await _context.Movies
                .Include(m => m.Screenings)
                .Include(m => m.MovieGenres)
                .ThenInclude(m=>m.Genre)
                .Where(m => m.Screenings.Any(s => s.ScreeningTime.Date >= currentDate))
                .ToListAsync();
        }

        public async Task<Movie> GetMovieByIdAsync(int id) => 
            await _context.Movies
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
                .Include(m => m.Screenings)
                .Include(m => m.MovieGenres)
                .ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

    }
}

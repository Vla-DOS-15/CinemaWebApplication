using CinemaWebApplication.Models;

namespace CinemaWebApplication.Interfaces
{
    public interface IScreeningService
    {
        Task<IEnumerable<Movie>> GetUpcomingScreeningsAsync();
        Task<Movie> GetMovieByIdAsync(int id);

    }
}

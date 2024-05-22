using CinemaWebApplication.Models;
using CinemaWebApplication.ViewModels;

namespace CinemaWebApplication.Interfaces
{
    public interface IMoviesService
    {
        Task<Movie> Details(int? id);
        Task Create(MovieViewModel model);
        Task Edit(int id, MovieViewModel model);
        Task Delete(int id);
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CinemaWebApplication.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public float Rating { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public List<int> SelectedActors { get; set; }
        public List<int> SelectedGenres { get; set; }


        public List<SelectListItem> Actors { get; set; }
        public List<SelectListItem> Genres { get; set; }
    }
}

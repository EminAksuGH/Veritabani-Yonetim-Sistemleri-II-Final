using System.Collections.Generic;

namespace FilmKurdu.Models.ViewModel
{
    public class HomePageWM
    {
        public List<Movies> MoviesTable { get; set; }
        public List<Series> SeriesTable { get; set; }
        public List<Reviews> ReviewsTable { get; set; }
        public List<Users> UsersTable { get; internal set; }
        public List<Watchlist> WatchlistTable { get; set; }
    }
}
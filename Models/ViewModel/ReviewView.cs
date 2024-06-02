using System.Collections.Generic;

namespace FilmKurdu.Models.ViewModel
{
    public class ReviewView
    {
        public List<Reviews> Reviews { get; set; }
        
        public Movies Movie { get; set; }

        public List<Users> Users { get; set; }
    }
}
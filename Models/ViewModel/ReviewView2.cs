using System.Collections.Generic;

namespace FilmKurdu.Models.ViewModel
{
    public class ReviewView2
    {
        public List<Reviews2> Reviews { get; set; }
        
        public Series Series { get; set; }

        public List<Users> Users { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmKurdu.Models
{
    [Table("watchlist")]
    public class Watchlist
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }


        [ForeignKey("User")]
        public int UserID { get; set; }
        public virtual Users User { get; set; }

        [ForeignKey("Movie")]
        public int MovieID { get; set; }
        public virtual Movies Movie { get; set; }
    }
}

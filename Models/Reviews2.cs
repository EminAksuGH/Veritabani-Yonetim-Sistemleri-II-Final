using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmKurdu.Models
{
    [Table("reviews2")]
    public class Reviews2
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int userID { get; set; }

        public int seriesID { get; set; }
        public string Review { get; set; }
        public string ReviewText { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmKurdu.Models
{
    [Table("reviews")]
    public class Reviews
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int userID { get; set; }

        public int movieID { get; set; }
        public string Review { get; set; }
        public string ReviewText { get; set; }
    }
}
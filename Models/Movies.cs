using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmKurdu.Models
{
    [Table("Movies")]
    public class Movies
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(600)]
        public string Description { get; set; }

        [StringLength(20)]
        public string Genre { get; set; }

        [StringLength(20)]
        public string Genre2 { get; set; }
        
        public string Image { get; set; }

        public DateTime ReleaseDate { get; set; }
        
        public string Stars { get; set; }

        public float Score { get; set; }

    }
}
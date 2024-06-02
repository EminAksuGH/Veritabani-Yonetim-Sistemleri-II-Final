namespace FilmKurdu.Models.ViewModel
{
    public class MediaItem
    {
        public string Type { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Genre2 { get; set; }
        
        public string Image { get; set; }
        public string ReleaseDate { get; set; }
        
        public string Stars { get; set; }
        public float Score{ get; set; }
    }
}
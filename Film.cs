using System.ComponentModel.DataAnnotations;

namespace film.Models
{
    public class Film
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Film adı zorunludur")]
        [Display(Name = "Film Adı")]
        public string FilmAdi { get; set; }

        [Required(ErrorMessage = "Yönetmen adı zorunludur")]
        [Display(Name = "Yönetmen")]
        public string Yonetmen { get; set; }

        [Display(Name = "Çıkış Yılı")]
        public int CikisYili { get; set; }

        [Display(Name = "Film Türü")]
        public string FilmTuru { get; set; }

        [Display(Name = "IMDB Puanı")]
        [Range(0, 10, ErrorMessage = "IMDB puanı 0-10 arasında olmalıdır")]
        public decimal IMDBPuani { get; set; }

        [Display(Name = "İzlendi mi?")]
        public bool Izlendimi { get; set; }
    }
} 
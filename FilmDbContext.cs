using Microsoft.EntityFrameworkCore;

namespace film.Models
{
    public class FilmDbContext : DbContext
    {
        public FilmDbContext(DbContextOptions<FilmDbContext> options)
            : base(options)
        {
        }

        public DbSet<Film> Filmler { get; set; }
    }
} 
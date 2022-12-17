using MangaMu.Plugin.Models;
using Microsoft.EntityFrameworkCore;

namespace MangaMu.Plugin
{
    public class PluginDbContext : DbContext
    {
        private readonly string _connectionString;
        public PluginDbContext(string connectionString) : base()
        {
            _connectionString= connectionString;
        }

        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<MangaType> Types { get; set; }

        public DbSet<MangaAuthor> MangaAuthors { get; set; }
        public DbSet<MangaGenre> MangaGenres { get; set; }
        public DbSet<MangaStatus> MangaStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}

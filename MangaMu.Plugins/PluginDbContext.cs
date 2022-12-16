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
        //public DbSet<MangaCategory> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}

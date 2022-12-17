using MangaMu.Plugin;
using MangaMu.Plugin.Contracts;
using MangaMu.Plugin.Providers;
using Microsoft.EntityFrameworkCore;

namespace MangaMu.Test
{
    [TestFixture]
    public class Manga4LifeTest
    {
        private Manga4Life _plugin;
        private PluginDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _plugin = new Manga4Life();
            _dbContext = _plugin.DbContext;
            _dbContext.Database.ExecuteSqlRaw(@"
                DELETE FROM Mangas;
                DELETE FROM Authors;
                DELETE FROM Genres;
                DELETE FROM Types;
                DELETE FROM Chapters;
                DELETE FROM Statuses;
            ");
        }

        [Test]
        public void Crawl_Success()
        {
            var plugin = new Manga4Life();
            CrawlResult result = null;

            Assert.DoesNotThrow(() => {
                result = plugin.CrawlPage("http://localhost:8000/search.html");
            });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MangaList, Is.Not.Null);
        }

        [Test]
        public void UpdateDatabase_Success()
        {
            var plugin = new Manga4Life();
            var success = false;
            Assert.DoesNotThrow(() => {
                success = plugin.UpdateDatabase().Result;
            });
            Assert.That(success, Is.True);

            var mangas = _dbContext.Mangas.ToList();
            Assert.That(mangas, Is.Not.Empty.And.Count.AtLeast(1000));
        }
    }
}

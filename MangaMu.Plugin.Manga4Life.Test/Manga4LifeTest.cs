using MangaMu.Plugin.Providers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Plug = MangaMu.Plugin.Providers.Manga4Life;

namespace MangaMu.Plugin.Manga4Life.Test
{
    [TestFixture]
    public class Manga4LifeTest
    {
        private PluginDriver _driver;
        private bool _keepDb => true;

        private string _rootPath;
        private string _searchPath;

        [OneTimeSetUp]
        public void SetUp()
        {
            _rootPath = Path.GetDirectoryName(GetType().Assembly.Location);

            if (_keepDb) DeleteLibFiles();
            else DeletePluginDir();

            _driver = new PluginDriver(false);

            _searchPath = Path.Combine(_rootPath, "Sample/search.html");

            using (var plugin = new Plug()) {
                plugin.EnsureDbCreated();

                var dbContext = plugin.DbContext;
                dbContext.Database.ExecuteSqlRaw(@"
                    DELETE FROM Mangas;
                    DELETE FROM Authors;
                    DELETE FROM Genres;
                    DELETE FROM Types;
                    DELETE FROM Chapters;
                    DELETE FROM Statuses;
                ");
            }

            CopyLibFiles();
        }

        [Test]
        public void LoadPlugins_Success()
        {
            var plugins = _driver.LoadFromPaths(new[] { "Plugins/Manga4Life" });

            Assert.That(plugins.Count, Is.EqualTo(1));
            Assert.That(plugins.First().Name, Is.EqualTo("Manga4Life"));
        }

        [Test]
        public void Crawl_Success()
        {
            var plugin = new Plug();
            CrawlResult result = null;

            Assert.DoesNotThrow(() => {
                using Stream stream = File.Open(_searchPath, FileMode.Open);
                result = plugin.ExtractResponseStream(stream);
            });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MangaList, Is.Not.Null);
        }

        [Test]
        public void UpdateDatabase_Success()
        {
            using var plugin = new Plug();
            var dbContext = plugin.DbContext;
            var success = false;
            Assert.Multiple(() => {
                Assert.DoesNotThrow(() => {
                    using Stream stream = File.Open(_searchPath, FileMode.Open);
                    var crawlResult = plugin.ExtractResponseStream(stream);
                    plugin.UpsertFilters(crawlResult.CrawlFilter);
                    plugin.UpsertMangas(crawlResult.MangaList);
                    success = true;
                });
                Assert.That(success, Is.True);

                var totalManga = dbContext.Mangas.Count();
                Assert.That(totalManga, Is.AtLeast(1000));
                Assert.That(dbContext.MangaAuthors.Count(), Is.AtLeast(totalManga));
                Assert.That(dbContext.MangaGenres.Count(), Is.AtLeast(totalManga));
            });
        }

        private void CopyLibFiles()
        {
            string fullName = "", pluginDir = "";
            using (var plugin = new Plug()) {
                fullName = plugin.GetType().Assembly.FullName.Split(',').First();
                pluginDir = Path.Combine(_rootPath, "Plugins/Manga4Life");
                if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);
            }
            var targetDllFile = Path.Combine(pluginDir, $"{fullName}.dll");
            var targetPdbFile = Path.Combine(pluginDir, $"{fullName}.pdb");
            File.Copy(Path.Combine(_rootPath, $"{fullName}.dll"), targetDllFile);
            File.Copy(Path.Combine(_rootPath, $"{fullName}.pdb"), targetPdbFile);
        }

        private void DeletePluginDir()
        {
            var pluginDir = Path.Combine(_rootPath, "Plugins");
            if (!Directory.Exists(pluginDir)) return;
            Directory.Delete(pluginDir, true);
        }

        private void DeleteLibFiles()
        {
            var fileName = typeof(Plug).Assembly.FullName.Split(',').First();
            var files = new DirectoryInfo(Path.Combine(_rootPath, "Plugins", "Manga4Life")).GetFiles(fileName + "*");
            foreach (var file in files) file.Delete();
        }
    }
}

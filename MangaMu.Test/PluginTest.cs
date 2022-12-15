using MangaMu.Plugin;

namespace MangaMu.Test
{
    public class Tests
    {
        private PluginDriver _driver;

        [SetUp]
        public void Setup()
        {
            _driver= new PluginDriver(false);
        }

        [Test]
        public void LoadPlugins_01_Success()
        {
            var plugins = _driver.LoadFromPaths(new[] { "Plugins" });

            Assert.That(plugins.Count, Is.EqualTo(2));
            Assert.That(plugins.First().Name, Is.EqualTo("Manga4Life"));
            Assert.That(plugins.Last().Name, Is.EqualTo("MangaDex"));
        }

        [Test]
        public void LoadPlugins_02_Success()
        {
            var plugins = _driver.LoadFromPaths(new[] { "Plugins/Manga4Life" });

            Assert.That(plugins.Count, Is.EqualTo(1));
            Assert.That(plugins.First().Name, Is.EqualTo("Manga4Life"));
        }

        [Test]
        public void LoadPlugins_03_Success()
        {
            var plugins = _driver.LoadFromPaths(new[] { "Plugins/MangaDex" });

            Assert.That(plugins.Count, Is.EqualTo(1));
            Assert.That(plugins.First().Name, Is.EqualTo("MangaDex"));
        }
    }
}

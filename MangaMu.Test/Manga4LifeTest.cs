using MangaMu.Plugin.Contracts;
using MangaMu.Plugin.Providers;

namespace MangaMu.Test
{
    [TestFixture]
    public class Manga4LifeTest
    {
        [Test]
        public void Crawl_Success()
        {
            var plugin = new Manga4Life();
            IEnumerable<IMangaInfo> items = new List<IMangaInfo>();

            Assert.DoesNotThrow(() => {
                items = plugin.CrawlPage("http://localhost:8000/search.html");
            });

            Assert.That(items, Is.Not.Empty);
        }
    }
}

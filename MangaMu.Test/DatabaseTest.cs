using MangaMu.Plugin.Providers;

namespace MangaMu.Test
{
    [TestFixture]
    public class DatabaseTest
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void CreateDbContext_Success()
        {
            Assert.DoesNotThrow(() => {
                var plugin = new Manga4Life();
                plugin.EnsureDbCreated();
                plugin.GetMangaList();
            });
        }
    }
}

using Flurl.Http;
using MangaMu.Plugin.Contracts;

namespace MangaMu.Plugin.Providers
{
    public class Manga4Life : PluginBase
    {
        public override string Name => "Manga4Life";
        public override string LogoUrl => "https://manga4life.com/media/favicon.png";
        private string _pageUrl => "";

        public override IEnumerable<IChapter> GetChapters()
        {
            throw new NotImplementedException();
        }

        public override IMangaInfo GetInfo(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> UpdateDatabase()
        {
        }

        private IEnumerable<IMangaInfo> CrawlPage()
        {
            var jObj = "";
            using (var stream = LogoUrl.GetStreamAsync().Result) {
                using var reader = new StreamReader(stream);
                while (!reader.EndOfStream) {
                    var currLine = reader.ReadLine().TrimStart();
                    if (!currLine.StartsWith("vm.Directory")) continue;
                    jObj = currLine;
                    break;
                }
            }

            var 
        }
    }
}

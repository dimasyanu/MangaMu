using MangaMu.Plugin.Contracts;

namespace MangaMu.Plugin.Providers
{
    public class MangaDex : PluginBase
    {
        public override string Name => "MangaDex";

        public override string LogoUrl => "https://mangadex.org/favicon.svg";

        public override IEnumerable<IChapter> GetChapters()
        {
            throw new NotImplementedException();
        }

        public override IMangaInfo GetInfo(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IMangaInfo> GetMangaList()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> UpdateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
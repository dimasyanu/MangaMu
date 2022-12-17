using MangaMu.Plugin.Contracts;

namespace MangaMu.Plugin.Providers
{
    public class MangaDex : PluginBase
    {
        public override string Name => "MangaDex";

        public override string LogoUrl => "https://mangadex.org/favicon.svg";

        public override IEnumerable<IChapter> GetChapters(Guid mangaId)
        {
            throw new NotImplementedException();
        }

        public override IManga GetMangaInfo(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IManga> GetMangaList()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> UpdateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
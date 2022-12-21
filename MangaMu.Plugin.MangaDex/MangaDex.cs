using MangaMu.Plugin.Contracts;
using MangaMu.Plugin.Models;

namespace MangaMu.Plugin.Providers
{
    public class MangaDex : PluginBase
    {
        public override string Name => "MangaDex";

        public override string LogoUrl => "https://mangadex.org/favicon.svg";
        public override string DbFileName => "MangaDex.db";

        public override IEnumerable<IChapter> GetChapters(Guid mangaId)
        {
            throw new NotImplementedException();
        }

        public override Manga GetMangaInfo(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Manga> GetMangaList()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> UpdateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
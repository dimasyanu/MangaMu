using MangaMu.Plugin.Models;

namespace MangaMu.Plugin.Contracts
{
    public interface IPlugin
    {
        string Name { get; }
        string LogoUrl { get; }

        Task<bool> UpdateDatabase();
        IEnumerable<IChapter> GetChapters(Guid mangaId);
        Manga GetMangaInfo(Guid id);
    }
}

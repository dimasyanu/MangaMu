namespace MangaMu.Plugin.Contracts
{
    public interface IPlugin
    {
        string Name { get; }
        string LogoUrl { get; }

        Task<bool> UpdateDatabase();
        IEnumerable<IChapter> GetChapters();
        IMangaInfo GetInfo(Guid id);
    }
}

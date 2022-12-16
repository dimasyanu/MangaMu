using MangaMu.Plugin.Contracts;

namespace MangaMu.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        public abstract string Name { get; }
        public abstract string LogoUrl { get; }

        public abstract IEnumerable<IMangaInfo> GetMangaList();
        public abstract IEnumerable<IChapter> GetChapters();
        public abstract IMangaInfo GetInfo(Guid id);
        public abstract Task<bool> UpdateDatabase();

        public readonly string DbFilePath;

        public PluginBase()
        {
            var baseDirPath = Path.GetDirectoryName(GetType().Assembly.Location);
            var dbDirPath = Path.Combine(baseDirPath, "Plugins", Name);
            DbFilePath = Path.Combine(dbDirPath, $"data.db");

            if (!Directory.Exists(dbDirPath)) Directory.CreateDirectory(dbDirPath);

            if (!File.Exists(DbFilePath)) File.Create(DbFilePath);
        }
    }
}

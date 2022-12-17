using MangaMu.Plugin.Contracts;
using MangaMu.Plugin.Models;
using Slugify;

namespace MangaMu.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        public abstract string Name { get; }
        public abstract string LogoUrl { get; }

        public abstract IEnumerable<Manga> GetMangaList();
        public abstract IEnumerable<IChapter> GetChapters(Guid mangaId);
        public abstract Manga GetMangaInfo(Guid id);
        public abstract Task<bool> UpdateDatabase();

        public readonly string DbFilePath;
        protected readonly SlugHelper SlugHelper;

        public PluginBase()
        {
            SlugHelper = new SlugHelper();
            var baseDirPath = Path.GetDirectoryName(GetType().Assembly.Location);
            var dbDirPath = Path.Combine(baseDirPath, "Plugins", Name);
            DbFilePath = Path.Combine(dbDirPath, $"data.db");

            if (!Directory.Exists(dbDirPath)) Directory.CreateDirectory(dbDirPath);

            if (!File.Exists(DbFilePath)) File.Create(DbFilePath);
        }
    }
}

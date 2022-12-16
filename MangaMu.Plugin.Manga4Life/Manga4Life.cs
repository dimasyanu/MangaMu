using Flurl.Http;
using MangaMu.Plugin.Contracts;
using MangaMu.Plugin.Manga4Life;
using MangaMu.Plugin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MangaMu.Plugin.Providers
{
    public class Manga4Life : PluginBase, IDisposable
    {
        public override string Name => "Manga4Life";
        public override string LogoUrl => "https://manga4life.com/media/favicon.png";

        private string _pageUrl => "http://localhost:8000/search.html";
        private string _connectionString => $"Data Source={DbFilePath};Cache=Shared";

        private readonly PluginDbContext _dbContext;

        public Manga4Life() : base()
        {
            _dbContext = new PluginDbContext(_connectionString);
        }

        public override IEnumerable<IMangaInfo> GetMangaList()
        {
            return _dbContext.Mangas.ToList();
        }

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
            var items = CrawlPage();
            return Task.FromResult(true);
        }

        private IEnumerable<IMangaInfo> CrawlPage() => CrawlPage(_pageUrl);

        public IEnumerable<IMangaInfo> CrawlPage(string url)
        {
            var jsonStr = "";
            using (var stream = url.GetStreamAsync().Result) {
                using var reader = new StreamReader(stream);
                while (!reader.EndOfStream) {
                    var currLine = reader.ReadLine().TrimStart();
                    if (!currLine.StartsWith("vm.Directory")) continue;

                    for (var i = 0; true; i++) {
                        var currChar = currLine[i];
                        if (currChar != '=') continue;
                        jsonStr = currLine
                            .Substring(i + 1)
                            .TrimStart()
                            .TrimEnd(';');
                        break;
                    }
                    break;
                }
            }

            var objects = JsonConvert.DeserializeObject<IEnumerable<MangaSourceDto>>(jsonStr);
            var results = objects.Select(x => new Manga {
                Name = x.I,
                Key = x.V,
                Authors = string.Join(", ", x.A),
            }).ToList();
            return results;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void EnsureDbCreated() => _dbContext.Database.EnsureCreated();
    }
}

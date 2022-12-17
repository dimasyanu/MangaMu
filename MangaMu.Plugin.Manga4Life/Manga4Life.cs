using Flurl.Http;
using MangaMu.Plugin.Contracts;
using MangaMu.Plugin.Manga4Life;
using MangaMu.Plugin.Models;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace MangaMu.Plugin.Providers
{
    public class Manga4Life : PluginBase
    {
        public override string Name => "Manga4Life";
        public override string LogoUrl => "https://manga4life.com/media/favicon.png";

        private string _pageUrl => "http://localhost:8000/search.html";
        private string _connectionString => $"Data Source={DbFilePath};Cache=Shared";

        public readonly PluginDbContext DbContext;

        public Manga4Life() : base()
        {
            DbContext = new PluginDbContext(_connectionString);
        }

        public override IEnumerable<Manga> GetMangaList()
        {
            return DbContext.Mangas.ToList();
        }

        public override IEnumerable<IChapter> GetChapters(Guid mangaId)
        {
            return DbContext.Mangas.First(x => x.Id == mangaId).Chapters;
        }

        public override Manga GetMangaInfo(Guid id)
        {
            return DbContext.Mangas.Find(id);
        }

        public override Task<bool> UpdateDatabase()
        {
            var crawlResult = CrawlPage();

            UpsertFilters(crawlResult.CrawlFilter);
            UpsertMangas(crawlResult.MangaList);

            return Task.FromResult(true);
        }

        private void UpsertFilters(CrawlFilter filters)
        {
            var newGenres = new List<Genre>();
            var newStatuses = new List<Status>();
            var newTypes = new List<Models.MangaType>();
            var dbGenres = DbContext.Genres.ToList();
            var dbStatuses = DbContext.Statuses.ToList();
            var dbTypes = DbContext.Types.ToList();

            foreach(var genre in filters.Genre) {
                if (dbGenres.Any(x => x.Name.ToLower() == genre.ToLower())) continue;
                newGenres.Add(new Genre { Name = genre, Alias = SlugHelper.GenerateSlug(genre) });
            }
            if (newGenres.Any()) DbContext.Genres.AddRange(newGenres);

            foreach(var type in filters.Type) {
                if (dbTypes.Any(x => x.Name.ToLower() == type.ToLower())) continue;
                newTypes.Add(new Models.MangaType { Name = type, Alias = SlugHelper.GenerateSlug(type) });
            }
            if (newTypes.Any()) DbContext.Types.AddRange(newTypes);

            foreach(var status in filters.PublishStatus) {
                if (dbStatuses.Any(x => x.Name.ToLower() == status.ToLower())) continue;
                newStatuses.Add(new Status { Name = status, Alias = SlugHelper.GenerateSlug(status) });
            }
            if (newStatuses.Any()) DbContext.Statuses.AddRange(newStatuses);

            var success = DbContext.SaveChanges();
            Console.WriteLine(success);
        }

        private void UpsertMangas(IEnumerable<MangaSourceDto> mangas)
        {
            var dbMangas = DbContext.Mangas.ToList();
            var dbAuthors = DbContext.Authors.ToList();
            var dbGenres = DbContext.Genres.ToList();
            var dbStatuses = DbContext.Statuses.ToList();
            var dbTypes = DbContext.Types.ToList();

            var newMangas = new List<Manga>();
            var newAuthors = new List<Author>();

            var distinctAuthors = mangas.SelectMany(x => x.A).Distinct();
            foreach (var author in distinctAuthors) {
                if (dbAuthors.Any(x => x.Name == author)) continue;
                newAuthors.Add(new Author { Name = author });
            }
            if (newAuthors.Any()) DbContext.Authors.AddRange(newAuthors);

            foreach (var item in mangas) {
                var type = dbTypes.FirstOrDefault(x => x.Name == item.T);
                var status = dbStatuses.FirstOrDefault(x => x.Name == item.Ps);
                var dbManga = dbMangas.FirstOrDefault(x => x.Key == item.Lt);

                // Update
                if (dbManga != null) {
                    dbManga.Name = item.S;
                    dbManga.Alias = item.I;
                    dbManga.ImageUrl = $"https://temp.compsci88.com/cover/{item.I}.jpg";
                    dbManga.Type = type;
                    dbManga.Status = status;
                    dbManga.UpdatedAt = DateTime.Now;
                }
                // Create
                else {
                    dbManga = new Manga {
                        Name = item.S,
                        Alias = item.I,
                        ImageUrl = $"https://temp.compsci88.com/cover/{item.I}.jpg",
                        Key = item.Lt,
                        Type = type,
                        Status = status,
                        PublishedAt = item.Ls == "0" ? null : DateTime.Parse(item.Ls),
                        UpdatedAt = DateTime.Now,
                    };
                    newMangas.Add(dbManga);
                }

                var genres = item.G;
                if (genres.Any()) {
                    var dbMangaGenres = DbContext.MangaGenres.Where(x => x.MangaId == dbManga.Id).ToList();
                    var newMangaGenres = new List<MangaGenre>();
                    foreach(var genre in genres) {
                        if (dbMangaGenres.Any(x => x.Genre.Name == genre)) continue;
                        var dbGenre = DbContext.Genres.First(x => x.Name == genre);
                        newMangaGenres.Add(new MangaGenre { MangaId = dbManga.Id, GenreId = dbGenre.Id });
                    }
                    if (newMangaGenres.Any()) DbContext.MangaGenres.AddRange(newMangaGenres);
                }

                var authors = item.A;
                if (authors.Any()) {
                    var dbMangaAuthors = DbContext.MangaAuthors.Where(x => x.MangaId == dbManga.Id).ToList();
                    var newMangaAuthors = new List<MangaAuthor>();
                    foreach(var author in authors) {
                        if (dbMangaAuthors.Any(x => x.Author.Name == author)) continue;
                        var dbAuthor = newAuthors.First(x => x.Name == author);
                        newMangaAuthors.Add(new MangaAuthor { MangaId = dbManga.Id, AuthorId = dbAuthor.Id });
                    }
                    if (newMangaAuthors.Any()) DbContext.MangaAuthors.AddRange(newMangaAuthors);
                }
            }
            if (newMangas.Any()) DbContext.Mangas.AddRange(newMangas);

            DbContext.SaveChanges();
        }

        private CrawlResult CrawlPage() => CrawlPage(_pageUrl);

        public CrawlResult CrawlPage(string url)
        {
            IEnumerable<MangaSourceDto> mangaList = new List<MangaSourceDto>();
            CrawlFilter filters = null;
            using (var stream = url.GetStreamAsync().Result) {
                using var reader = new StreamReader(stream);
                while (!reader.EndOfStream) {
                    var currLine = reader.ReadLine().TrimStart();

                    if (currLine.StartsWith("vm.Directory")) {
                        mangaList = ExtractMangaList(currLine);
                        continue;
                    }

                    if (currLine.StartsWith("vm.AvailableFilters")) {
                        var jsonStr = currLine;
                        while(currLine != "};") {
                            currLine = reader.ReadLine().TrimStart().TrimEnd();
                            jsonStr += $"\n{currLine}";
                        }
                        jsonStr.TrimEnd(';');

                        filters = ExtractFilters(jsonStr);
                        continue;
                    }
                }
            }

            var results = new CrawlResult {
                CrawlFilter = filters,
                MangaList = mangaList
            };
            return results;
        }

        public IEnumerable<MangaSourceDto> ExtractMangaList(string lineStr)
        {
            var listJsonStr = "";
            for (var i = 0; true; i++) {
                var currChar = lineStr[i];
                if (currChar != '=') continue;
                listJsonStr = lineStr
                    .Substring(i + 1)
                    .TrimStart()
                    .TrimEnd(';');
                break;
            }
            var listDto = JsonConvert.DeserializeObject<IEnumerable<MangaSourceDto>>(listJsonStr);
            return listDto.Where(x => x.Lt != 0);
        }

        public CrawlFilter ExtractFilters(string lineStr)
        {
            var listJsonStr = "";
            for (var i = 0; true; i++) {
                var currChar = lineStr[i];
                if (currChar != '=') continue;
                listJsonStr = lineStr
                    .Substring(i + 1)
                    .TrimStart()
                    .TrimEnd(';');
                break;
            }
            var listDto = JsonConvert.DeserializeObject<CrawlFilter>(listJsonStr);
            return listDto;
        }

        public void EnsureDbCreated() => DbContext.Database.EnsureCreated();
    }

    public class CrawlResult
    {
        public IEnumerable<MangaSourceDto> MangaList { get; set; }
        public CrawlFilter CrawlFilter { get; set; }
    }

    public class CrawlFilter
    {
        public IEnumerable<string> PublishStatus { get; set; }
        public IEnumerable<string> ScanStatus { get; set; }
        public IEnumerable<string> Type { get; set; }
        public IEnumerable<string> Genre { get; set; }
    }
}

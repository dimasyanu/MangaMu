using Flurl.Http;
using MangaMu.Plugin.Contracts;
using MangaMu.Plugin.Manga4Life;
using MangaMu.Plugin.Models;
using Newtonsoft.Json;

namespace MangaMu.Plugin.Providers
{
    public class Manga4Life : PluginBase, IDisposable
    {
        public override string Name => "Manga4Life";
        public override string LogoUrl => "https://manga4life.com/media/favicon.png";
        public override string DbFileName => "Manga4Life.db";

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

        internal void UpsertFilters(CrawlFilter filters)
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

        internal void UpsertMangas(IEnumerable<MangaSourceDto> mangas)
        {
            var dbMangas = DbContext.Mangas.ToList();
            var dbAuthors = DbContext.Authors.ToList();
            var dbGenres = DbContext.Genres.ToList();
            var dbStatuses = DbContext.Statuses.ToList();
            var dbTypes = DbContext.Types.ToList();
            var allMangaGenres = DbContext.MangaGenres.ToList();
            var allMangaAuthors = DbContext.MangaAuthors.ToList();

            var newMangas = new List<Manga>();
            var newAuthors = new List<Author>();
            var newMangaAuthors = new List<MangaAuthor>();
            var newMangaGenres = new List<MangaGenre>();

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
                    var dbMangaGenres = allMangaGenres.Where(x => x.MangaId == dbManga.Id).ToList();
                    var currMangaGenres = new List<MangaGenre>();
                    foreach(var genre in genres) {
                        if (dbMangaGenres.Any(x => x.Genre.Name == genre)) continue;
                        var dbGenre = dbGenres.First(x => x.Name == genre);
                        currMangaGenres.Add(new MangaGenre { MangaId = dbManga.Id, GenreId = dbGenre.Id });
                    }
                    if (currMangaGenres.Any()) newMangaGenres.AddRange(currMangaGenres);
                }

                var authors = item.A;
                if (authors.Any()) {
                    var dbMangaAuthors = allMangaAuthors.Where(x => x.MangaId == dbManga.Id).ToList();
                    var currMangaAuthors = new List<MangaAuthor>();
                    foreach(var author in authors) {
                        if (dbMangaAuthors.Any(x => x.Author.Name == author)) continue;
                        var dbAuthor = newAuthors.First(x => x.Name == author);
                        currMangaAuthors.Add(new MangaAuthor { MangaId = dbManga.Id, AuthorId = dbAuthor.Id });
                    }
                    if (currMangaAuthors.Any()) newMangaAuthors.AddRange(currMangaAuthors);
                }
            }
            if (newMangas.Any()) DbContext.Mangas.AddRange(newMangas);
            if (newMangaGenres.Any()) DbContext.MangaGenres.AddRange(newMangaGenres);
            if (newMangaAuthors.Any()) DbContext.MangaAuthors.AddRange(newMangaAuthors);

            DbContext.SaveChanges();
        }

        private CrawlResult CrawlPage() => CrawlPage(_pageUrl);

        internal CrawlResult CrawlPage(string url)
        {
            using var stream = url.GetStreamAsync().Result;
            return ExtractResponseStream(stream);
        }

        internal CrawlResult ExtractResponseStream(Stream stream)
        {
            IEnumerable<MangaSourceDto> mangaList = new List<MangaSourceDto>();
            CrawlFilter filters = null;

            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream) {
                var currLine = reader.ReadLine().TrimStart();

                if (currLine.StartsWith("vm.Directory")) {
                    mangaList = ExtractMangaList(currLine);
                    continue;
                }

                if (currLine.StartsWith("vm.AvailableFilters")) {
                    var jsonStr = currLine;
                    while (currLine != "};") {
                        currLine = reader.ReadLine().TrimStart().TrimEnd();
                        jsonStr += $"\n{currLine}";
                    }
                    jsonStr.TrimEnd(';');

                    filters = ExtractFilters(jsonStr);
                    continue;
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

        public void Dispose()
        {
            DbContext.Dispose();
        }
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

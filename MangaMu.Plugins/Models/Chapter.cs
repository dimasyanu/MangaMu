using MangaMu.Plugin.Contracts;

namespace MangaMu.Plugin.Models
{
    public class Chapter : IChapter
    {
        public Guid Id { get; set; }
        public Guid MangaId { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual Manga Manga { get; set; }
    }
}

namespace MangaMu.Plugin.Models
{
    public class Author
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        
        public virtual ICollection<Manga> Mangas { get; set; } = new List<Manga>();
    }
}

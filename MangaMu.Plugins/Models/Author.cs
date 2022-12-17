namespace MangaMu.Plugin.Models
{
    public class Author
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        
        // Many to many
        public virtual ICollection<MangaAuthor> MangaAuthors { get; set; } = new List<MangaAuthor>();
    }
}

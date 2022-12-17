using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaMu.Plugin.Models
{
    public class Manga
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [StringLength(256)]
        public string Alias { get; set; } = string.Empty;

        [StringLength(256)]
        public long Key { get; set; }

        [ForeignKey("Type")]
        public Guid TypeId { get; set; }

        [ForeignKey("Status")]
        public Guid StatusId { get; set; }

        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual MangaType Type { get; set; }
        public virtual Status Status { get; set; }

        // One to many
        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

        // Many to many
        public virtual ICollection<MangaAuthor> MangaAuthors { get; set; } = new List<MangaAuthor>();
        public virtual ICollection<MangaGenre> MangaGenres { get; set; } = new List<MangaGenre>();
    }
}

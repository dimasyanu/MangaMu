using MangaMu.Plugin.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaMu.Plugin.Models
{
    public class Manga : IManga
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [StringLength(256)]
        public string Key { get; set; } = string.Empty;

        [ForeignKey("Type")]
        public Guid TypeId { get; set; }

        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual MangaType Type { get; set; }
        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}

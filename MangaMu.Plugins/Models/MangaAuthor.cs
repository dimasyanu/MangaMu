using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaMu.Plugin.Models
{
    public class MangaAuthor
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Manga")]
        public Guid MangaId { get; set; }

        [ForeignKey("Author")]
        public Guid AuthorId { get; set; }

        public virtual Manga Manga { get; set; }
        public virtual Author Author { get; set; }
    }
}

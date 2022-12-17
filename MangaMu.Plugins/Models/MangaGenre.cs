using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MangaMu.Plugin.Models
{
    public class MangaGenre
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Manga")]
        public Guid MangaId { get; set; }

        [ForeignKey("Genre")]
        public Guid GenreId { get; set; }

        public virtual Manga Manga { get; set; }
        public virtual Genre Genre { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MangaMu.Plugin.Models
{
    public class MangaStatus
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Manga")]
        public Guid MangaId { get; set; }

        [ForeignKey("Status")]
        public Guid StatusId { get; set; }

        public virtual Manga Manga { get; set; }
        public virtual Status Status { get; set; }
    }
}

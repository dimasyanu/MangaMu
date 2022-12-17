using System.ComponentModel.DataAnnotations;

namespace MangaMu.Plugin.Models
{
    public class Status
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;

        public virtual ICollection<Manga> Mangas { get; set; } = new List<Manga>();
    }
}

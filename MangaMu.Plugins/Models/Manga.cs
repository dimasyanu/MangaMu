using MangaMu.Plugin.Contracts;
using System.ComponentModel.DataAnnotations;

namespace MangaMu.Plugin.Models
{
    public class Manga : IMangaInfo
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [StringLength(256)]
        public string Key { get; set; } = string.Empty;

        [StringLength(256)]
        public string Authors { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

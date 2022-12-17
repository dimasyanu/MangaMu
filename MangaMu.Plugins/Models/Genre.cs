﻿using System.ComponentModel.DataAnnotations;

namespace MangaMu.Plugin.Models
{
    public class Genre
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;

        public virtual ICollection<MangaGenre> MangasGenres { get; set; } = new List<MangaGenre>();
    }
}

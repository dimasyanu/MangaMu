namespace MangaMu.Plugin.Manga4Life
{
    public class MangaSourceDto
    {
        public string H { get; set; } = string.Empty;
        public string I { get; set; } = string.Empty;
        public string L { get; set; } = string.Empty;
        public long Lt { get; set; }
        public string Ls { get; set; } = string.Empty;
        public string O { get; set; } = string.Empty;
        public string Ps { get; set; } = string.Empty;
        public string S { get; set; } = string.Empty;
        public string Ss { get; set; } = string.Empty;
        public string T { get; set; } = string.Empty;
        public string V { get; set; } = string.Empty;
        public string Vm { get; set; } = string.Empty;
        public string Y { get; set; } = string.Empty;

        public IEnumerable<string> A { get; set; } = new List<string>();
        public IEnumerable<string> Al { get; set; } = new List<string>();
        public IEnumerable<string> G { get; set; } = new List<string>();

    }
}

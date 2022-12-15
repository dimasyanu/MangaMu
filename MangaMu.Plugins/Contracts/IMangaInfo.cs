namespace MangaMu.Plugin.Contracts
{
    public interface IMangaInfo
    {
        string Name { get; set; }
        string Description { get; set; }
        string Author { get; set; }
        string ImageUrl { get; set; }
    }
}

namespace MangaMu.Plugin.Contracts
{
    public interface IMangaInfo
    {
        string Name { get; set; }
        string Description { get; set; }
        string Authors { get; set; }
        string ImageUrl { get; set; }
    }
}

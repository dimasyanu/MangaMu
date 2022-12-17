using MangaMu.Plugin.Models;

namespace MangaMu.Plugin.Contracts
{
    public interface IManga
    {
        string Name { get; set; }
        string Description { get; set; }
        ICollection<Author> Authors { get; set; }
        string ImageUrl { get; set; }
    }
}

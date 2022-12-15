using MangaMu.Contracts;

namespace MangaMu.Models
{
    public class PluginListItem : IPluginListItem
    {
        public string Id { get ; set ; } = string.Empty;
        public string Name { get ; set ; } = string.Empty;
        public string LogoUrl { get ; set ; } = string.Empty;
    }
}

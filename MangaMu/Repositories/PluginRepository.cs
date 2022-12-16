using MangaMu.Contracts;
using MangaMu.Plugin;
using MangaMu.Plugin.Models;

namespace MangaMu.Repositories
{
    public class PluginRepository
    {
        private readonly PluginDriver _pluginDriver;

        public PluginRepository(PluginDriver pluginDriver)
        {
            _pluginDriver = pluginDriver;
        }

        public async Task UpdateDatabase(string pluginId)
        {
            var success = await _pluginDriver.UpdateDatabase(pluginId);
            if (!success) throw new OperationCanceledException("Update database failed");
        }

        public IEnumerable<IPluginListItem> GetPlugins()
        {
            var pluginList = _pluginDriver.GetPlugins();
            var results = pluginList.Select(x => {
                return new PluginListItem {
                    Id = x.Name,
                    Name = x?.Name ?? "",
                    LogoUrl = x?.LogoUrl ?? ""
                };
            });
            return results;
        }
    }
}

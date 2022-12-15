using MangaMu.Plugin;
using MangaMu.Repositories;

namespace MangaMu
{
    public static class DependencyInjection
    {
        public static void RegisterDependencies(this WebApplicationBuilder builder)
        {
            var pluginDriver = new PluginDriver(new[] {
                "../../../../MangaMu.Plugin.Manga4Life/bin/Debug/net6.0",
                "../../../../MangaMu.Plugin.MangaDex/bin/Debug/net6.0"
            });

            builder.Services.AddSingleton(pluginDriver);

            builder.Services.AddMvc();
            builder.Services.AddControllers();
            builder.Services.AddScoped<PluginRepository>();

            // Register plugins
            var plugins = pluginDriver.GetPlugins().ToList();
            plugins.ForEach(plugin => builder.Services.AddSingleton(plugin));
        }
    }
}

using MangaMu.Plugin.Contracts;
using System.Reflection;
using System.Runtime.Loader;

namespace MangaMu.Plugin
{
    public class PluginDriver
    {
        private readonly IEnumerable<string> _pluginPaths = new[] {
            "Plugins"
        };

        private readonly IEnumerable<IPlugin> _plugins;

        public PluginDriver(bool autoLoad = true)
        {
            if (!autoLoad) return;
            _plugins = LoadFromPaths(_pluginPaths);
        }

        public PluginDriver(IEnumerable<string> pluginPaths, bool autoLoad = true)
        {
            _pluginPaths = pluginPaths;
            if (!autoLoad) return;
            _plugins = LoadFromPaths(_pluginPaths);
        }

        public IEnumerable<IPlugin> LoadFromPaths(IEnumerable<string> pluginPaths)
            => pluginPaths.SelectMany(path => {
                    var pluginAssemblies = LoadAssembly(path);
                    var results = new List<IPlugin>();
                    foreach (var assembly in pluginAssemblies) {
                        results.AddRange(GetPluginsFromAssembly(assembly));
                    }
                    return results;
                });

        private IEnumerable<IPlugin> GetPluginsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(x => x.Namespace.StartsWith("MangaMu.Plugin"));
            var results = types
                .Select(type => {
                    if (!typeof(IPlugin).IsAssignableFrom(type)) return null;
                    return Activator.CreateInstance(type) as IPlugin;
                })
                .Where(x => x != null);

            if (!results.Any()) {
                string availableTypes = string.Join(",", types.Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }

            return results;
        }

        private IEnumerable<Assembly> LoadAssembly(string relativePath)
        {
            var assemblyPath = Path.Combine(
                Path.GetDirectoryName(GetType().Assembly.Location),
                relativePath.Replace('\\', Path.DirectorySeparatorChar)
            );

            var paths = ScanFolderForAssemblies(assemblyPath);

            var results = new List<Assembly>();
            foreach (var path in paths) {
                string pluginLocation = Path.GetFullPath(path);
                var contextLoader = new PluginLoadContext(pluginLocation);
                var assembly = contextLoader.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
                results.Add(assembly);
            }
            return results;
        }

        private IEnumerable<string> ScanFolderForAssemblies(string folder)
        {
            var results = new List<string>();

            // Check for sub-directories
            var subDirs = Directory.GetDirectories(folder);
            if (subDirs.Any()) {
                foreach (var subDir in subDirs) {
                    results.AddRange(ScanFolderForAssemblies(subDir));
                }
            }

            var dlls = Directory.GetFiles(folder, "*.dll")
                .Where(x => x.Split('/').Last().Split('\\').Last().StartsWith("MangaMu.Plugin."));
            
            results.AddRange(dlls);
            return results;
        }

        public IEnumerable<IPlugin> GetPlugins() => _plugins;

        public async Task<bool> UpdateDatabase(string pluginId)
        {
            var type = typeof(IPlugin).Assembly
                .GetExportedTypes()
                .FirstOrDefault(x => string.Equals(x.FullName, $"MangaMu.Plugin.Providers.{pluginId}", StringComparison.Ordinal));
            if (type == null) throw new OperationCanceledException("Plugin not installed.");

            var plugin = Activator.CreateInstance(type) as IPlugin;

            return await plugin.UpdateDatabase();
        }
    }

    class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null) {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null) {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}

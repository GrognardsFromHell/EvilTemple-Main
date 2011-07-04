using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EvilTemple.Runtime;
using System.Linq;

namespace EvilTemple.Support
{
    /// <summary>
    /// Performs discovery of game modules.
    /// </summary>
    public class GameModuleLoader
    {

        private readonly GameModules _gameModules;

        private readonly IPaths _paths;

        private readonly IUserSettings _userSettings;

        public GameModuleLoader(GameModules gameModules, IPaths paths, IUserSettings userSettings)
        {
            _gameModules = gameModules;
            _userSettings = userSettings;
            _paths = paths;
        }

        public void LoadAllModules()
        {
            var discoveredModules = DiscoverModules();

            _gameModules.LoadModules(discoveredModules);
        }

        /// <summary>
        /// Discovers all loadable modules.
        /// </summary>
        /// <returns>A set of paths to modules that seem eglible for loading.</returns>
        private IEnumerable<string> DiscoverModules()
        {
            Trace.TraceInformation("Discovering loadable modules.");

            var result = new HashSet<string>();

            // Discover all modules in the local data path
            var zipFiles = Directory.EnumerateFiles(_paths.GeneratedDataPath, "*.zip");
            foreach (var zipFile in zipFiles)
                DiscoverModule(zipFile, result);

            zipFiles = Directory.EnumerateFiles(_paths.InstallationPath, "*.zip");
            foreach (var zipFile in zipFiles)
                DiscoverModule(zipFile, result);

            return result;
        }

        private void DiscoverModule(string path, ICollection<string> result)
        {
            Trace.TraceInformation("Discovering module {0}", path);

            var manifest = GameModuleManifest.Read(path);

            // Skip files without a manifest
            if (manifest == null)
            {
                Trace.TraceInformation("Skipping zip file without manifest: {0}", path);
                return;
            }

            // Is the module's id specifically disabled?
            var disabledModules = _userSettings.GetOrSet(@"disabledModules", new HashSet<string>());
            if (disabledModules.Contains(manifest.Id))
            {
                Trace.TraceInformation("Skipping {0}, because the contained module {1} has been disabled by the user.", path, manifest.Id);
                return;
            }

            // TODO: Check whether the core version matches the requested core version.

            // TODO: Signature validation

            result.Add(path);
        }
    }
}

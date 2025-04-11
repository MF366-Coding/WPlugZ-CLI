using System.IO;
using WPlugZ_CLI.Source;


namespace WPlugZ_CLI.Plugin
{

    public class VersionDeleter
    {

        string name;
        int specificVersion;
        string workingDir;

        /// <summary>
        /// Instantiate the class PluginDeleter, capable of safely deleting only specific WPlugZ-CLI projects' versions, as to not disturb the other ones.
        /// </summary>
        /// <param name="name">The name of the plugin whose version you wish to remove</param>
        /// <param name="version">The specific version of the plugin to remove</param>
        /// <param name="cwd">The working directory</param>
        public VersionDeleter(string name, int version, string cwd)
        {

            this.name = name;
            specificVersion = version;
            workingDir = cwd;

        }

        bool IsVersionWithinLimits()
        {
            return specificVersion >= 1 & specificVersion <= 1000;
        }

        string IsValidPlugin()
        {
            string plausiblePath = Path.Join(workingDir, name);
            return Directory.Exists(plausiblePath) ? plausiblePath : null;
        }

        string IsValidVersion()
        {
            string plausiblePath = Path.Join(workingDir, name, $"v{specificVersion}");
            return Directory.Exists(plausiblePath) ? plausiblePath : null;
        }

        /// <summary>
        /// Delete the specified version.
        /// </summary>
        /// <returns>Return code (0 = success, 3 = invalid plugin, 4 = invalid version)</returns>
        public int DeleteEverything()
        {

            string pluginPath = IsValidPlugin();
            if (pluginPath == null) return 3; // [i] the plugin itself is not valid

            bool versionWithinLimits = IsVersionWithinLimits();
            string versionPath = IsValidVersion();
            if (!versionWithinLimits | versionPath == null) return 4; // [i] the version is not valid

            OSEnvironment.DeleteDirectory(versionPath);

            return 0;

        }

    }

    public class PluginDeleter
    {

        string name;
        string workingDir;

        /// <summary>
        /// Instantiate the class PluginDeleter, capable of safely deleting WriterClassic plugins as WPlugZ-CLI projects.
        /// </summary>
        /// <param name="name">The name of the plugin to remove.</param>
        /// <param name="cwd">The working directory</param>
        public PluginDeleter(string name, string cwd)
        {

            this.name = name;
            workingDir = cwd;

        }

        string IsValidPlugin()
        {

            string plausiblePath = Path.Join(workingDir, name);
            return Directory.Exists(plausiblePath) ? plausiblePath : null;

        }

        /// <summary>
        /// Deletes the whole plugin and whatever you have inside the plugin folder.
        /// </summary>
        /// <returns>Retunr code (0 = success, 3 = invalid plugin)</returns>
        public int DeleteEverything()
        {

            string pluginPath = IsValidPlugin();
            if (pluginPath == null) return 3; // [i] the plugin itself is not valid

            OSEnvironment.DeleteDirectory(pluginPath);

            return 0;

        }


    }

}

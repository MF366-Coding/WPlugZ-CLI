using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WPlugZ_CLI.Source;


namespace WPlugZ_CLI.Plugin
{

    public class Bumper
    {

        string name;
        int version;
        string workingDir;
        string author;
        string description;
        string iconPath;

        public Bumper(string name, int version, string author, string description, string iconPath, string cwd)
        {

            this.name = name;
            this.version = version;
            this.author = author;
            this.description = description;
            this.iconPath = iconPath;
            workingDir = cwd;

        }

        int IsVersionWithinLimits()
        {

            if (version < 1) return 2;
            if (version >= 1 & version <= 1000) return 1;
            return 0;

        }

        string IsValidPlugin()
        {
            string plausiblePath = Path.Join(workingDir, name);
            return Directory.Exists(plausiblePath) ? plausiblePath : null;
        }

        string GetLatestVersion(string pluginPath)
        {
            string latest = null;
            foreach (string directory in Directory.EnumerateDirectories(pluginPath).Reverse())
            {

                if (!Regex.IsMatch(Path.GetFileName(directory), @"^v(1000|[1-9][0-9]{0,2})$"))
                {
                    continue;
                }                
                if (latest == null)
                {
                    latest = directory;
                    continue;
                }
                if (Convert.ToInt32(Path.GetFileName(directory).AsSpan(1).ToString()) == 1000) return directory; // [i] can't go bigger than that
                if (Convert.ToInt32(Path.GetFileName(latest).AsSpan(1).ToString()) < Convert.ToInt32(Path.GetFileName(directory).AsSpan(1).ToString()))
                {
                    latest = directory; // [i] considered the latest version
                }

            }

            return latest;
        }

        private bool IsValidIconFile()
        {

            string[] VALID_ICONTYPES = {".png", ".jpg", ".gif", ".jfif"};
            return File.Exists(iconPath) & (VALID_ICONTYPES.Contains(Path.GetExtension(iconPath).Normalize()));

        }

        bool DoesVersionExist()
        {
            return Directory.Exists(Path.Join(workingDir, name, $"v{version}"));
        }

        /// <summary>
        /// Creates the new version's directory.
        /// </summary>
        /// <returns>
        /// Return Code
        /// - 0: Successful
        /// - 5: Plugin is invalid
        /// - 6: Version is not within limits
        /// - 7: Latest version is 1000
        /// - 8: Version to bump to already exists
        /// </returns>
        public int CreateNewVersionDirectory()
        {

            string pluginPath = IsValidPlugin();
            if (pluginPath == null) return 5;

            int versionLimit = IsVersionWithinLimits();
            if (versionLimit == 0) return 6;
            if (versionLimit == 2)
            {

                string latestPath = GetLatestVersion(pluginPath);
                ReadOnlySpan<char> latestVersion = Path.GetFileName(latestPath).AsSpan(1);
                short latestVersionNumber = Convert.ToInt16(latestVersion.ToString());

                if (latestVersionNumber == 1000) return 7;
                version = latestVersionNumber + 1;
            }

            if (DoesVersionExist()) return 8;
            Directory.CreateDirectory(Path.Join(workingDir, name, $"v{version}"));

            return 0;
        
        }

        /// <summary>
        /// Copies the specified icon file to the correct location.
        /// </summary>
        /// <returns>The path of the copy</returns>
        public string CopyImageFileToCorrectLocation()
        {

            if (!IsValidIconFile()) return null;

            string newIconPath = Path.Join(workingDir, name, $"v{version}", "WriterPlugin.png");
            
            try
            {
                File.Copy(iconPath, newIconPath);
            }
            catch (Exception)
            {
                File.Delete(newIconPath);
                File.Copy(iconPath, newIconPath);
            }

            return newIconPath;

        }

        /// <summary>
        /// Creates the plugin's Details.txt file
        /// </summary>
        public void CreateDetailsFile()
        {

            File.WriteAllText(Path.Join(workingDir, name, $"v{version}", "Details.txt"), $"{name}\n{author}\n{description}");

        }

        /// <summary>
        /// Creates a placeholder Python file for the plugin.
        /// </summary>
        /// <returns>The path to the newly created Python file.</returns>
        public string CreatePlaceholderPythonFile()
        {

            string pythonFilePath = Path.Join(workingDir, name, $"v{version}", $"{name}.py");
            File.WriteAllText(pythonFilePath, "from typing import Any\n\ndef start(_globals: dict[str, Any]):\n    print('Hello, World!')\n    return _globals\n");
            return pythonFilePath;

        }

        /// <summary>
        /// Update the MANIFEST file to contain the "bumped" version.
        /// </summary>
        /// <param name="pathToIcon">The path to the icon file</param>
        /// <param name="pathToPythonFile">The path to the Python file</param>
        /// <returns>Return Code (0 = success, 9 = manifest file does not exist)</returns>
        public int UpdateManifestFile(string pathToIcon, string pathToPythonFile)
        {

            string manifestPath = Path.Join(workingDir, name, "manifest.json");
            if (!File.Exists(manifestPath)) return 9;

            Dictionary<string, string> manifestData = new()
            {
                { "name", name },
                { "author", author },
                { "description", description },
                { "imagefile", pathToIcon },
                { "pyfile", pathToPythonFile }
            };

            var manifestDataAsObject = manifestData.ToDictionary(
                pair => pair.Key,
                pair => (object)pair.Value
            );

            JSON manifestAsJson = new JSON()
                                            .LoadFromFile(manifestPath)
                                            .Set($"v{version}", manifestDataAsObject);
            
            manifestAsJson.Save(manifestPath);
            return 0;

        }

        /// <summary>
        /// Remove debris if necessary.
        /// Removes the directory created by CreateNewVersionDirectory and everything inside.
        /// </summary>
        /// <returns>Return Code (0 = debris removed successfully; anything else = error occured)</returns>
        public int RemoveDebris()
        {

            // [i] just say the word and it's gone
            return OSEnvironment.DeleteDirectory(Path.Join(workingDir, name, $"v{version}"));

        }

    }

}
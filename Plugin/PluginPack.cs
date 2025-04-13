using System;
using System.IO;
using System.IO.Compression;


namespace WPlugZ_CLI.Plugin
{

    public class PackHelper
    {

        string name;
        int version;
        string saveLocation;
        string workingDir;
        bool useZipExtension;
        CompressionLevel compressionLevel;

        public PackHelper(string name, int version, string saveLocation, bool useZipExtension, CompressionLevel compressionLevel, string cwd)
        {

            this.name = name;
            this.version = version;
            this.saveLocation = saveLocation;
            this.useZipExtension = useZipExtension;
            this.compressionLevel = compressionLevel;
            workingDir = cwd;

        }

        bool IsVersionWithinLimits()
        {

            return version >= 1 & version <= 1000;

        }

        string IsValidPlugin()
        {
            string plausiblePath = Path.Join(workingDir, name);
            return Directory.Exists(plausiblePath) ? plausiblePath : null;
        }

        bool IsValidSaveLocation()
        {
            return Directory.Exists(saveLocation);
        }

        string DoesVersionExist(string pluginPath)
        {
            string plausiblePath = Path.Join(pluginPath, $"v{version}");
            return Directory.Exists(plausiblePath) ? plausiblePath : null;
        }
        
        string GetRequestedExtension()
        {
            return useZipExtension == true ? "zip" : "wplugz";
        }

        void ZipFolder(string versionPath)
        {

            using (ZipArchive zipFile = ZipFile.Open(Path.Join(saveLocation, $"{name}_v{version}.{GetRequestedExtension()}"), ZipArchiveMode.Update))
            {

                var filesToPack = Directory.GetFiles(versionPath, "*", SearchOption.AllDirectories);

                foreach (var file in filesToPack)
                {

                    string entryName = Path.GetRelativePath(Path.GetDirectoryName(versionPath)!, file);
                    zipFile.CreateEntryFromFile(file, entryName, compressionLevel);

                }

            }

        }

        /// <summary>
        /// Zips a plugin's version.
        /// </summary>
        /// <returns>
        /// Return Code:
        /// - 0: Success
        /// - 10: Version is not within limits
        /// - 11: Invalid plugin
        /// - 12: Save location is invalid
        /// - 13: Invalid version
        /// - 14: Error occured while zipping the directory
        /// </returns>
        public int PackPluginVersion()
        {

            if (!IsVersionWithinLimits()) return 10;
            
            string pluginPath = IsValidPlugin();
            if (pluginPath == null) return 11;
            
            if (saveLocation != null)
            {
                if (!IsValidSaveLocation()) return 12;
            }
            else
            {
                saveLocation = workingDir;
            }

            string versionPath = DoesVersionExist(pluginPath);
            if (versionPath == null) return 13;

            try
            {
                ZipFolder(versionPath);
                return 0;
            }
            catch (Exception)
            {
                return 14;
            }

        }

        public string WorkingDirectory
        {
            get { return workingDir; }
        }

        public string SaveLocation
        {
            get { return saveLocation; }
            set { saveLocation = value; }
        }


    }

}

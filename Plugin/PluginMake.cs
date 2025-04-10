using System;
using System.IO;
using System.Linq;
using WPlugZ_CLI.Source;


namespace WPlugZ_CLI.Plugin
{

    public class Creator
    {

        public string name;
        public string author;
        public string description;
        public string iconPath;
        public string workingDir;
        int version;
        public bool authorfile;
        public bool versioning;
        public bool readme;

        /// <summary>
        /// Instantiate the class Creator, capable of creating WriterClassic plugins as WPlugZ-CLI projects.
        /// </summary>
        /// <param name="name">The name of the plugin</param>
        /// <param name="author">The name of the plugin's maintainer/creator/owner</param>
        /// <param name="description">The description of the plugin</param>
        /// <param name="iconPath">The path to the plugin's icon</param>
        /// <param name="workingDirectory">The directory to create the plugin in</param>
        /// <param name="version">The plugin's initial version, as a non-clamped integer</param>
        /// <param name="authorfile">Whether to create an AUTHORFILE or not</param>
        /// <param name="versioning">Whether to create a VERSIONING file or not</param>
        /// <param name="readme">Whether to create a README file or not</param>
        public Creator(string name, string author, string description, string iconPath, string workingDirectory, int version, bool authorfile, bool versioning, bool readme)
        {

            this.name = name;
            this.author = author;
            this.description = description;
            this.iconPath = iconPath;
            workingDir = workingDirectory; // [i] no ambiguity = no need for 'this'
            this.version = version;
            this.authorfile = authorfile;
            this.versioning = versioning;
            this.readme = readme;

        }

        private bool IsValidWorkingDirectory()
        {

            return Directory.Exists(workingDir);

        }

        private bool IsValidIconFile()
        {

            string[] VALID_ICONTYPES = {".png", ".jpg", ".gif", ".jfif"};
            return File.Exists(iconPath) & (VALID_ICONTYPES.Contains(Path.GetExtension(iconPath).Normalize()));

        }

        /// <summary>
        /// Clamps the specified plugin version between 1 and 1000.
        /// </summary>
        public void ClampVersion()
        {

            version = Numbers.ClampInteger(version, 1, 1000);

        }

        /// <summary>
        /// Creates the plugin directory and the version subdirectory.
        /// </summary>
        /// <returns>Return code (0 = success; 1 = working directory is invalid; 2 = an error occured)</returns>
        public int CreatePluginDirectory()
        {

            if (!IsValidWorkingDirectory()) return 1;
            
            try
            {

                var dir = Directory.CreateDirectory(Path.Join(workingDir, name, $"v{version}"));
                return 0;


            }
            catch (Exception)
            {
                return 2;
            }

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
        /// Creates the MANIFEST file for the plugin.
        /// </summary>
        /// <param name="pathToIcon">The path to the icon (not the user one, it's the copied one)</param>
        /// <param name="pathToPythonFile">The path to the Python file created with CreatePlaceholderPythonFile</param>
        public void CreateManifestFile(string pathToIcon, string pathToPythonFile)
        {
            string jsonAsString = $@"
            {{
                ""v{version}"": {{
                    ""name"": ""{name}"",
                    ""author"": ""{author}"",
                    ""description"": ""{description}"",
                    ""imagefile"": ""{pathToIcon}"",
                    ""pyfile"": ""{pathToPythonFile}""
                }}
            }}";

            JSON manifest = new JSON();
            manifest.Load(jsonAsString);
            manifest.Save(Path.Join(workingDir, name, "manifest.json"));

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
        /// <returns></returns>
        public string CreatePlaceholderPythonFile()
        {

            string pythonFilePath = Path.Join(workingDir, name, $"v{version}", $"{name}.py");
            File.WriteAllText(pythonFilePath, "from typing import Any\n\ndef start(_globals: dict[str, Any]):\n    print('Hello, World!')\n    return _globals\n");
            return pythonFilePath;

        }

        private void CreateReadme()
        {

            File.WriteAllText(
                Path.Join(workingDir, name, "README.md"),
                $"# {name}\n{description}\n\nMade with <3 by {author}\n"
            );

        }

        private void CreateAuthorfile()
        {

            File.WriteAllText(
                Path.Join(workingDir, name, "AUTHOR.md"),
                $"Plugin maintained by: {author}\n"
            );

        }

        private void CreateVersioningFile()
        {

            File.WriteAllText(
                Path.Join(workingDir, name, "VERSIONING.md"),
                $"v{version}: Initial Release\n"
            );

        }

        /// <summary>
        /// Creates the additional optional files.
        /// </summary>
        public void CreateAdditionalFiles()
        {

            if (versioning)
            {
                CreateVersioningFile();
            }

            if (readme)
            {
                CreateReadme();
            }

            if (authorfile)
            {
                CreateAuthorfile();
            }

        }

        /// <summary>
        /// Retrieves the plugin's version.
        /// </summary>
        public int PluginVersion
        {

            get { return version; }

        }

    }

}


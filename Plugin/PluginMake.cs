using System;
using System.IO;
using System.Text.Json;


namespace WPlugZ_CLI.Plugin
{

    public class Creator
    {

        public string name;
        public string author;
        public string description;
        public string iconPath;
        public string workingDir;
        public int version;
        public bool authorfile;
        public bool versioning;
        public bool readme;

        /// <summary>
        /// Instantiate the class Creator, capable of creating WriterClassic plugins as WPlugZ-CLI porjects.
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
            workingDir = workingDirectory; // no ambiguity = no need for 'this'
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
            return File.Exists(iconPath);// TODO: & (VALID_ICONTYPES. Path.GetExtension(iconPath).Normalize() {});

        }

        private void ClampVersion()
        {

            if (version < 1)
            {
                version = 1;
                return;
            }

            version = version > 1001 ? 1000 : version;

        }

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

        public void CopyImageFile()
        {

            // TODO
            return;

        }

        public void CreateManifestFile()
        {
            string jsonAsString = $@"
            {{
                ""v{version}"": {{
                    ""name"": ""{name}"",
                    ""author"": ""{author}"",
                    ""description"": ""{description}""
                }}
            }}";

            using JsonDocument document = JsonDocument.Parse(jsonAsString);
            // TODO
            

        }

    }

}


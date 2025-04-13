using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using WPlugZ_CLI.Source;


namespace WPlugZ_CLI.Plugin
{

    public class Tester
    {

        string name;
        int version;
        string writerClassicPath;
        string workingDir;
        bool forceOverwrite;

        public Tester(string name, int version, string wClassicPath, bool force, string cwd)
        {

            this.name = name;
            this.version = version;
            writerClassicPath = wClassicPath;
            forceOverwrite = force;
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

        string IsValidWriterClassicPath()
        {
            if (!File.Exists(writerClassicPath)) return null;
            string parentDir = Path.GetDirectoryName(writerClassicPath);
            if (parentDir == null) return null;
            string plausiblePath = Path.Join(parentDir!, "plugins");
            return plausiblePath ?? null;
        }

        string DoesVersionExist(string pluginPath)
        {
            string plausiblePath = Path.Join(pluginPath, $"v{version}");
            return Directory.Exists(plausiblePath) ? plausiblePath : null;
        }

        bool CopyPluginToWCPlDir(string versionPath, string wCPluginDir)
        {

            // [!!] This will always overwrite plugin 1000
            // [i] I made this choice cuz I mean who the fuck has 1000 plugins???
            string plugin1000 = Path.Join(wCPluginDir, "plugin_1000");
            
            if (Directory.Exists(plugin1000)) 
            {

                if (!forceOverwrite) return false;
                OSEnvironment.DeleteDirectory(plugin1000);

            }

            Directory.CreateDirectory(plugin1000);
            
            foreach (string file in Directory.GetFiles(versionPath, "*", SearchOption.AllDirectories))
            {

                string entryName = Path.GetRelativePath(versionPath!, file);
                
                if (entryName.Split(Path.DirectorySeparatorChar).Length > 1)
                {

                    string dirPart = Path.GetDirectoryName(entryName)!;
                    string filePart = Path.GetFileName(entryName);
                    Directory.CreateDirectory(Path.Join(plugin1000, dirPart));
                    File.Copy(file, Path.Join(plugin1000, dirPart, filePart));

                }

                else
                {

                    File.Copy(file, Path.Join(plugin1000, entryName));

                }

            }

            return true;

        }

        /// <summary>
        /// Starts the test.
        /// </summary>
        /// <returns>
        /// Return Code
        /// - 0: Success
        /// - 15: Version is not within limits
        /// - 16: Invalid plugin
        /// - 17: Invalid version
        /// - 18: Invalid WriterClassic path
        /// - 19: Failed to copy (directory already exists - use --force/--overwrite/-F)
        /// - 20: Error running WriterClassic
        /// </returns>
        public int InitiateTest()
        {

            if (!IsVersionWithinLimits()) return 15;

            string pluginPath = IsValidPlugin();
            if (pluginPath == null) return 16;

            string versionPath = DoesVersionExist(pluginPath);
            if (versionPath == null) return 17;

            string wCPluginDir = IsValidWriterClassicPath();
            if (wCPluginDir == null) return 18;

            bool copied = CopyPluginToWCPlDir(versionPath, wCPluginDir);
            if (!copied) return 19;

            return 0;
        }

        /// <summary>
        /// Run the process.
        /// </summary>
        /// <returns>
        /// Process Exit Code or -7 (local error)
        /// </returns>
        public int RunProcess()
        {
            try
            {

                ProcessStartInfo psi = new()
                {

                    FileName = writerClassicPath.EndsWith(".py") ? OSEnvironment.PythonDenominator : writerClassicPath,
                    Arguments = writerClassicPath.EndsWith(".py") ? writerClassicPath : "",
                    RedirectStandardOutput = false,
                    UseShellExecute = true

                };

                using (Process test = Process.Start(psi))
                {

                    test.WaitForExit();
                    return test.ExitCode;

                }
                
            }

            catch (Exception) { return -7; }

        } 

    }

}


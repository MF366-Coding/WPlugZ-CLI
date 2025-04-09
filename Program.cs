using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.CommandLine;
using WPlugZ_CLI.Source;
using WPlugZ_CLI.Plugin;
using System.Reflection.Metadata.Ecma335;


namespace WPlugZ_CLI
{

    class Program
    {

        /// <summary>True if the current WPlugZ-CLI version is a pre-release or a "dev version"</summary>
        static bool isDevVersion = false;

        /// <summary>The current WPlugZ-CLI version</summary>
        static readonly string VERSION = "v3.1.0";
        static string latestVersion = null;
        public static readonly string OWNER = "MF366-Coding";
        public static readonly string PROJECT_NAME = "WPlugZ-CLI";
        public static readonly string PLUGIN_API_URL = "https://github.com/MF366-Coding/WriterClassic/wiki/Plugin-API-(v10.1.1-)";

        /// <summary>The GitHub API endpoint where the latest version data is</summary>
        static readonly string API_URL = $"https://api.github.com/repos/{OWNER}/{PROJECT_NAME}/releases/latest";
        
        static readonly HttpClient CLIENT = new HttpClient();


        /// <summary>
        /// Gets WPlugZ's latest version from the GitHub API and saves it into variable latestVersion.
        /// </summary>
        static async void GetLatestVersion()
        {

            try
            {

                CLIENT.DefaultRequestHeaders.UserAgent.TryParseAdd("request"); // [i] GitHub shit

                var response = await CLIENT.GetAsync(API_URL);
                response.EnsureSuccessStatusCode(); // [<] Ya better pray to God, for those sweet 200 success codes

                using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                latestVersion = document.RootElement.GetProperty("tag_name").GetString();

            }
            catch (Exception)
            {

                latestVersion = null;

            }

        }

        /// <summary>
        /// Checks if the current version is the latest one.
        /// </summary>
        /// <returns>True if the current version matches; false otherwise.</returns>
        static bool IsLatestVersion()
        {

            if (latestVersion == null) return false;

            if (latestVersion.EndsWith("-dev"))
            {

                isDevVersion = true;
            
            };

            return latestVersion == VERSION;

        }

        /// <summary>
        /// Compares current version of WPlugZ with the latest one available.
        /// </summary>
        static void CompareVersions()
        {

            GetLatestVersion();
            bool isLatest = IsLatestVersion();
            
            if (isLatest)
            {

                Console.WriteLine("Your WPlugZ-CLI is up-to-date.");
            
            }

            else
            {

                Console.WriteLine("Your WPlugZ-CLI might be out-of-date or no info could be gathered on versioning.");

            }

        }

        public static async Task Main(string[] args)
        {

            var rootCommand = new RootCommand("WPlugZ Plugin Manager CLI");

            // [i] The only 2 global options we really have
            var disableUpdateCheckOption = new Option<bool> (

                aliases: new[] { "--disable-update-check", "--dchkupd" },
                description: "Disable update checks on startup",
                getDefaultValue: () => false

            );

            var accessPluginDocsOption = new Option<bool> (

                aliases: new[] { "--info", "-w" },
                description: "Open the online Plugin API documentation",
                getDefaultValue: () => false

            );

            rootCommand.AddGlobalOption(disableUpdateCheckOption);
            rootCommand.AddGlobalOption(accessPluginDocsOption);

            ////////////// [*] //////////////
            ////// [*]  Create command //////
            ////////////// [*] //////////////
            var createCommand = new Command("new", "Create a new WriterClassic plugin");
            var pluginNameMkArg = new Argument<string>("name", "The name of the plugin to create");
            var pluginAuthorOpt = new Option<string> (

                aliases: new[] { "--author", "-a" },
                description: "The creator and maintainer of the plugin",
                getDefaultValue: () => OSEnvironment.GetUsername()

            );
            var pluginDescriptionOpt = new Option<string> (

                aliases: new[] { "--description", "-d" },
                description: "The description of the plugin to create",
                getDefaultValue: () => "WriterClassic Plugin"

            );
            var pluginIconOpt = new Option<string> (

                aliases: new[] { "--icon", "-i" },
                description: "The icon of the plugin to create",
                getDefaultValue: () => Path.Join(AppContext.BaseDirectory, "WriterPlugin.png")

            );
            var workingDirectoryOpt = new Option<string> (

                aliases: new[] { "--workingdir", "-D" },
                description: "Set the working directory for the plugin",
                getDefaultValue: () => Environment.CurrentDirectory

            );
            var pluginVersionOpt = new Option<int> (

                aliases: new[] { "--version", "-v" },
                description: "The starting version of the plugin to create",
                getDefaultValue: () => 1

            );
            var doFullCreationOpt = new Option<bool> (

                aliases: new[] { "--full", "-f" },
                description: "Create the additonal optional files",
                getDefaultValue: () => false

            );
            createCommand.AddArgument(pluginNameMkArg);
            createCommand.AddOption(pluginAuthorOpt);
            createCommand.AddOption(pluginDescriptionOpt);
            createCommand.AddOption(pluginIconOpt);
            createCommand.AddOption(workingDirectoryOpt);
            createCommand.AddOption(pluginVersionOpt);
            createCommand.AddOption(doFullCreationOpt);

            createCommand.SetHandler(void (name, author, desc, icon, wd, version, fullCreation) =>
                {

                    Creator pluginCreator = new Creator(
                        name,
                        author,
                        desc,
                        icon,
                        wd,
                        version,
                        fullCreation, // [i] Creates an authorfile
                        fullCreation, // [i] Creates a versioning file
                        fullCreation // [i] Creates a README file
                    );

                    pluginCreator.ClampVersion();
                    Console.WriteLine($"Got version number {pluginCreator.PluginVersion}");

                    Console.WriteLine("Attempting to create plugin directory...");
                    int retCode = pluginCreator.CreatePluginDirectory();

                    switch (retCode)
                    {

                        case 1:
                            Console.WriteLine("The working directory you've specified is invalid.\nAborting...");
                            Environment.Exit(1);
                            break;

                        case 2:
                            Console.WriteLine("Failed to create the directory.\nAborting...");
                            Environment.Exit(1);
                            break;

                        default:
                            // [<] success, I guess!
                            break;
                        
                    }

                    Console.WriteLine($"Copying specified icon file to the correct location...");
                    string imgPath = pluginCreator.CopyImageFileToCorrectLocation();

                    Console.Write(imgPath == null ? "The specified icon file's path is invalid." : "Copied image to correct location!");
                    
                    if (imgPath == null)
                    {
                        Environment.Exit(2);
                    }

                    Console.Write("\n");
                    Console.WriteLine("Creating the placeholder Python file...");
                    string pyfilePath = pluginCreator.CreatePlaceholderPythonFile();

                    Console.WriteLine("Creating the Details.txt file...");
                    pluginCreator.CreateDetailsFile();

                    Console.WriteLine("Creating the MANIFEST file...");
                    pluginCreator.CreateManifestFile(imgPath, pyfilePath);

                    Console.WriteLine("Creating the additional files...");
                    pluginCreator.CreateAdditionalFiles();

                    Console.WriteLine("Done!");

                },
                pluginNameMkArg, pluginAuthorOpt, pluginDescriptionOpt, pluginIconOpt, workingDirectoryOpt, pluginVersionOpt, doFullCreationOpt);
            createCommand.AddAlias("create");
            rootCommand.AddCommand(createCommand);

            ////////////// [*] //////////////
            ////// [*]  Delete command //////
            ////////////// [*] //////////////
            var delCommand = new Command("delete", "Delete an existing WPlugZ project");
            var pluginNameDelArg = new Argument<string>("name", "The name of the plugin to remove");
            var skipConfirmationOpt = new Option<bool> (

                aliases: new[] { "--skip", "-S" },
                description: "Skip confirmation for file removal",
                getDefaultValue: () => false

            );
            delCommand.AddArgument(pluginNameDelArg);
            delCommand.AddOption(skipConfirmationOpt);

            delCommand.SetHandler(void (name, skip) =>
                {

                    Deleter pluginDeleter = new Deleter(
                        name,
                        skip
                    );

                    // TODO: implement deleter logic

                },
                pluginNameDelArg, skipConfirmationOpt);
            delCommand.AddAlias("remove");
            delCommand.AddAlias("rm");
            delCommand.AddAlias("del");
            rootCommand.AddCommand(delCommand);

            ////////////// [*] //////////////
            /////// [*]  Bump command ///////
            ////////////// [*] //////////////
            var bumpCommand = new Command("bump", "Bump a WriterClassic plugin");
            // TODO: implement BUMP logic

            ////////////// [*] //////////////
            /////// [*]  Test command ///////
            ////////////// [*] //////////////
            var testCommand = new Command("test", "Test a version of a WriterClassic plugin with WriterClassic");
            // TODO: implement TEST logic

            ////////////// [*] //////////////
            /// [*] Manifest verification ///
            ////////////// [*] //////////////
            var verifyCommand = new Command("verify", "Run a MANIFEST verification on a WriterClassic plugin");
            // TODO: implement Manifest verification logic
            verifyCommand.AddAlias("manifest");

            ////////////// [*] //////////////
            /////// [*]  Root Command ///////
            ////////////// [*] //////////////
            rootCommand.SetHandler(
                (bool disableUpdateCheck, bool accessPluginDocs) =>
                {

                    if (!disableUpdateCheck)
                    {

                        CompareVersions();

                    }

                    if (accessPluginDocs)
                    {

                        WebBrowser.OpenURL(PLUGIN_API_URL);
                        Environment.Exit(0);

                        

                    }

                },
                disableUpdateCheckOption, accessPluginDocsOption);

            await rootCommand.InvokeAsync(args);

        }

    }

}

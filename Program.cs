using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.CommandLine;
using WPlugZ_CLI.Source;
using WPlugZ_CLI.Plugin;


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
        
        static readonly HttpClient CLIENT = new();

        static Random randomizer = new();

        static bool allowColors = true; 

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

                Logger.SuccessLine("Your WPlugZ-CLI is up-to-date.", 2);
            
            }

            else
            {

                Logger.WarningLine("Your WPlugZ-CLI might be out-of-date or no info could be gathered on versioning.", 2);

            }

        }

        private static void ConfirmRemoval()
        {

            var key = Console.ReadKey();

            if (key.KeyChar != 'y')
            {
                Logger.ErrorLine("\nOperation cancelled by user input.", 0);
                Colors.ResetAllEffects();
                Environment.Exit(0);
            }

        }

        public static async Task Main(string[] args)
        {

            var rootCommand = new RootCommand("WPlugZ Plugin Manager CLI");

            // [i] The only 3 global options we really have
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
            var pluginAuthorMkOpt = new Option<string> (

                aliases: new[] { "--author", "-a" },
                description: "The creator and maintainer of the plugin",
                getDefaultValue: () => OSEnvironment.GetUsername()

            );
            var pluginDescriptionMkOpt = new Option<string> (

                aliases: new[] { "--description", "-d" },
                description: "The description of the plugin to create",
                getDefaultValue: () => "WriterClassic Plugin"

            );
            var pluginIconMkOpt = new Option<string> (

                aliases: new[] { "--icon", "-i" },
                description: "The icon of the plugin to create",
                getDefaultValue: () => Path.Join(AppContext.BaseDirectory, "Assets", "WriterPlugin.png")

            );
            var workingDirectoryOpt = new Option<string> (

                aliases: new[] { "--workingdir", "-D" },
                description: "Set the working directory for the plugin",
                getDefaultValue: () => Environment.CurrentDirectory

            );
            var pluginVersionMkOpt = new Option<int> (

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
            createCommand.AddOption(pluginAuthorMkOpt);
            createCommand.AddOption(pluginDescriptionMkOpt);
            createCommand.AddOption(pluginIconMkOpt);
            createCommand.AddOption(workingDirectoryOpt);
            createCommand.AddOption(pluginVersionMkOpt);
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
                    Logger.SuccessLine($"✔ Got version number {pluginCreator.PluginVersion}", 0);

                    Logger.InfoLine("➦ Attempting to create plugin directory...", 2);
                    int retCode = pluginCreator.CreatePluginDirectory();

                    switch (retCode)
                    {

                        case 1:
                            Logger.ErrorLine("✖ The working directory you've specified is invalid.\nAborting...");
                            Colors.ResetAllEffects();
                            Environment.Exit(1);
                            break;

                        case 2:
                            Logger.ErrorLine("✖ Failed to create the directory.\nAborting...");
                            Colors.ResetAllEffects();
                            Environment.Exit(1);
                            break;

                        default:
                            // [<] success, I guess!
                            break;
                        
                    }

                    Logger.InfoLine($"➦ Copying specified icon file to the correct location...");
                    string imgPath = pluginCreator.CopyImageFileToCorrectLocation();

                    if (imgPath == null)
                    {
                        Logger.ErrorLine("✖ The specified icon file's path is invalid.", 2);
                        Colors.ResetAllEffects();
                        Environment.Exit(2);
                    }
                    
                    Logger.SuccessLine("✔ Copied the icon to the correct location!", 2);
                    Logger.InfoLine("➦ Creating the placeholder Python file...");
                    string pyfilePath = pluginCreator.CreatePlaceholderPythonFile();

                    Logger.InfoLine("➦ Creating the Details.txt file...");
                    pluginCreator.CreateDetailsFile();

                    Logger.InfoLine("➦ Creating the MANIFEST file...");
                    pluginCreator.CreateManifestFile(imgPath, pyfilePath);

                    Logger.InfoLine("➦ Creating the additional files...", 0);
                    pluginCreator.CreateAdditionalFiles();

                    Logger.SuccessLine("✔ Done!", 0);

                },
                pluginNameMkArg, pluginAuthorMkOpt, pluginDescriptionMkOpt, pluginIconMkOpt, workingDirectoryOpt, pluginVersionMkOpt, doFullCreationOpt);
            createCommand.AddAlias("create");
            rootCommand.AddCommand(createCommand);


            ////////////// [*] //////////////
            ////// [*]  Delete command //////
            ////////////// [*] //////////////
            var delCommand = new Command("delete", "Delete an existing WPlugZ project");
            var pluginNameDelArg = new Argument<string>("name", "The name of the plugin to remove");
            var pluginVersionDelOpt = new Option<int> (

                aliases: new[] { "--version", "-v" },
                description: "The version of the plugin to remove (defaults to all)",
                getDefaultValue: () => -1

            );
            var skipConfirmationOpt = new Option<bool> (

                aliases: new[] { "--skip", "-S" },
                description: "Skip confirmation for file removal",
                getDefaultValue: () => false

            );
            delCommand.AddArgument(pluginNameDelArg);
            delCommand.AddOption(pluginVersionDelOpt);
            delCommand.AddOption(skipConfirmationOpt);

            delCommand.SetHandler(void (name, version, skip) =>
                {

                    string newlineChar = "";
                    
                    if (version < 1)
                    {
                        PluginDeleter pluginDeleter = new PluginDeleter(
                            name,
                            Environment.CurrentDirectory
                        );

                        if (!skip)
                        {

                            Logger.Warning($"⚠ Are you sure you wish to remove plugin {name}?\nThis action is irreversable and will wipe EVERYTHING inside the plugin's folder.\nContinue? (y/n) ");
                            ConfirmRemoval();
                            newlineChar = "\n";

                        }

                        Logger.InfoLine($"{newlineChar}➦ Initiating removal...", 2);
                        
                        int retCode = pluginDeleter.DeleteEverything();

                        if (retCode == 3)
                        {

                            Logger.ErrorLine("✖ No such plugin present in the current working directory.");
                            Colors.ResetAllEffects();
                            Environment.Exit(3); // [i] invalid plugin
                        
                        }

                        Logger.SuccessLine("✔ Removal finished!", 0);
                        Colors.ResetAllEffects();
                        Environment.Exit(0);
                    }

                    else
                    {
                        VersionDeleter versionDeleter = new VersionDeleter(
                            name,
                            version,
                            Environment.CurrentDirectory
                        );

                        if (!skip)
                        {

                            Logger.Warning($"⚠ Are you sure you wish to remove version {version} of plugin {name}?\nThis action is irreversable and will wipe EVERYTHING inside the version's folder.\nContinue? (y/n) ");
                            ConfirmRemoval();
                            newlineChar = "\n";

                        }

                        Logger.InfoLine($"{newlineChar}➦ Initiating removal...");
                        
                        int retCode = versionDeleter.DeleteEverything();

                        if (retCode == 3)
                        {
                            Logger.ErrorLine("✖ No such plugin present in the current working directory.");
                            Colors.ResetAllEffects();
                            Environment.Exit(3); // [i] invalid plugin
                        }
                        if (retCode == 4)
                        {
                            Logger.ErrorLine("✖ Plugin has no such version.", 2);
                            Colors.ResetAllEffects();
                            Environment.Exit(3); // [i] invalid version
                        }

                        Logger.SuccessLine("✔ Removal finished!", 0);
                        Colors.ResetAllEffects();
                        Environment.Exit(0);
                    }

                },
                pluginNameDelArg, pluginVersionDelOpt, skipConfirmationOpt);
            delCommand.AddAlias("remove");
            delCommand.AddAlias("rm");
            delCommand.AddAlias("del");
            rootCommand.AddCommand(delCommand);


            ////////////// [*] //////////////
            /////// [*]  Bump command ///////
            ////////////// [*] //////////////
            var bumpCommand = new Command("bump", "Bump a WriterClassic plugin");
            var pluginNameBmpArg = new Argument<string>("name", "The name of the plugin to bump");
            var pluginVersionBmpOpt = new Option<int> (

                aliases: new[] { "--version", "-v" },
                description: "The version of the plugin to bump to (defaults to one above latest)",
                getDefaultValue: () => -1

            );
            var pluginAuthorBmpOpt = new Option<string> (

                aliases: new[] { "--author", "-a" },
                description: "The creator and maintainer of the plugin",
                getDefaultValue: () => OSEnvironment.GetUsername()

            );
            var pluginDescriptionBmpOpt = new Option<string> (

                aliases: new[] { "--description", "-d" },
                description: "The description of the plugin to create",
                getDefaultValue: () => "WriterClassic Plugin"

            );
            var pluginIconBmpOpt = new Option<string> (

                aliases: new[] { "--icon", "-i" },
                description: "The icon of the plugin to create",
                getDefaultValue: () => Path.Join(AppContext.BaseDirectory, "Assets", "WriterPlugin.png")

            );

            bumpCommand.AddArgument(pluginNameBmpArg);
            bumpCommand.AddOption(pluginVersionBmpOpt);
            bumpCommand.AddOption(pluginAuthorBmpOpt);
            bumpCommand.AddOption(pluginDescriptionBmpOpt);
            bumpCommand.AddOption(pluginIconBmpOpt);

            bumpCommand.SetHandler(void (name, version, author, desc, icon) =>
                {

                    Bumper pluginBumper = new(name, version, author, desc, icon, Environment.CurrentDirectory);

                    Logger.SuccessLine("✔ Got correct version number.", 2);
                    Logger.InfoLine("➦ Creating the new version directory...", 0);
                    int retCode = pluginBumper.CreateNewVersionDirectory();

                    switch (retCode)
                    {

                        case 5:
                            Logger.ErrorLine("✖ The specified plugin is invalid!");
                            Environment.Exit(4);
                            break;

                        case 6:
                            Logger.ErrorLine("✖ The specified version must be between 1 and 1000, both ends included.", 0);
                            Environment.Exit(4);
                            break;

                        case 7:
                            Logger.ErrorLine("✖ The latest version found is v1000 (maximum reached).\nIf you are aware of the existance of versions below 1000 that are not occupied, feel free to use their slots by specifying the '-v' flag.", 1);
                            Environment.Exit(4);
                            break;

                        case 8:
                            Logger.ErrorLine("✖ The specified version already exists.");
                            Environment.Exit(4);
                            break;

                        default:
                            Logger.SuccessLine("✔ Created the version directory successfully.");
                            break;

                    }

                    Logger.InfoLine("➦ Creating Details.txt...");
                    pluginBumper.CreateDetailsFile();
                    Logger.SuccessLine("✔ Created Details.txt!");

                    Logger.InfoLine("➦ Creating the placeholder Python file...");
                    string pyfilePath = pluginBumper.CreatePlaceholderPythonFile();
                    Logger.SuccessLine("✔ Created the placeholder Python file!\nFeel free to replace it with your actual file after the bump operation terminates.");

                    Logger.InfoLine("➦ Copying the image to the correct location...");
                    string iconPath = pluginBumper.CopyImageFileToCorrectLocation();
                    
                    if (iconPath == null)
                    {

                        Logger.ErrorLine("✖ The specified image path is invalid.\nInitiating removal of debris...");
                        pluginBumper.RemoveDebris();
                        Logger.WarningLine("⚠ Debris removed! Bump operation exited with Error Code 4.", 0);
                        Environment.Exit(4);
                        return;

                    }

                    Logger.SuccessLine("✔ Copied the image to the correct location!");
                    Logger.InfoLine("➦ Updating the MANIFEST file...");
                    retCode = pluginBumper.UpdateManifestFile(iconPath, pyfilePath);

                    if (retCode == 9)
                    {

                        Logger.ErrorLine("✖ The MANIFEST file could not be found.\nInitiating removal of debris...");
                        pluginBumper.RemoveDebris();
                        Logger.WarningLine("⚠ Debris removed! Bump operation exited with Error Code 4.", 0);
                        Environment.Exit(4);
                        return;

                    }

                    Logger.SuccessLine("✔ Updated the MANIFEST!");
                    Logger.SuccessLine("✔ Done!", 0);


                },
                pluginNameBmpArg, pluginVersionBmpOpt, pluginAuthorBmpOpt, pluginDescriptionBmpOpt, pluginIconBmpOpt);
            bumpCommand.AddAlias("bmp");
            rootCommand.AddCommand(bumpCommand);


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
                        Colors.ResetAllEffects();
                        Environment.Exit(0);
                    }

                },
                disableUpdateCheckOption, accessPluginDocsOption);

            await rootCommand.InvokeAsync(args);

        }

    }

}

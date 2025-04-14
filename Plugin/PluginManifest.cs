using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Help;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using WPlugZ_CLI.Source;


namespace WPlugZ_CLI.Plugin
{

    public class ManifestAttention
    {

        int code;
        char identifier;
        string color;
        string errorType;

        public ManifestAttention(int code, char id, string color, string errorType)
        {

            this.code = code;
            identifier = Convert.ToChar(id.ToString().ToUpper());
            this.color = color;
            this.errorType = errorType;

        }

        protected string Identifier
        {

            get { return $"{identifier}{code} - "; }

        }

        protected string Type
        {

            get { return errorType; }

        }

        /// <summary>
        /// Prints a error message related to the manifest.
        /// </summary>
        /// <param name="reason">Why did the error happen?</param>
        public void PrintMessage(string reason)
        {

            Console.WriteLine($"{color}{Identifier}{Type}{Colors.STRONG_RESET} ({reason.Trim()})");

        }

    }

    public class ManifestError : ManifestAttention
    {

        public ManifestError(int code, string errorType)
            : base(code, 'E', Colors.RED, errorType) {}

    }

    public class ManifestWarning : ManifestAttention
    {

        public ManifestWarning(int code, string errorType)
            : base(code, 'W', Colors.YELLOW, errorType) {}

    }

    public class ManifestHint : ManifestAttention
    {
        readonly bool ignoreHints;
        readonly bool isReminder;

        public ManifestHint(int code, string errorType, bool ignoreHints = false, bool isReminder = false)
            : base(code, isReminder ? 'R' : 'H', Colors.CYAN, errorType) { this.ignoreHints = ignoreHints; this.isReminder = isReminder; }

        /// <summary>
        /// Prints a error message related to the manifest.
        /// If this is an hint and we're suppressing hints, nothing will be printed.
        /// </summary>
        /// <param name="reason">Why did the error happen?</param>
        public void PrintMessage(string reason)
        {

            if (ignoreHints & !isReminder) return;
            Console.WriteLine($"{Colors.CYAN}{Identifier}{Type}{Colors.STRONG_RESET} ({reason.Trim()})");

        }

    }

    public class ManifestHelper
    {

        string file;
        string[] placeholders;
        int hints = 0;
        int problems = 0;
        bool ignoreHints;
        JSON manifestJson;

        // [!] Manifest Errors
        ManifestError MGeneralError = new(0, "GeneralError");
        ManifestError MTypeError = new(1, "TypeError");
        ManifestError MInvalidVersionTag = new(2, "InvalidVersionTag");
        ManifestError MForbiddenName = new(3, "ForbiddenName");
        ManifestError MExpectedSomething = new(4, "EmptyVersionObject");
        ManifestError MMissingParameter = new(5, "MandatoryParameterMissing");

        // [!?] Manifest Warning
        ManifestWarning MGeneralWarning = new(0, "GeneralWarning");
        ManifestWarning MRedundantUseOfUncompatible = new(1, "UncompatibleByDefault");
        ManifestWarning MRedundantParameter = new(2, "RedundantUseOfParameter");
        ManifestWarning MLongParameter = new(3, "LongStringAsParameter");
        ManifestWarning MWrongName = new(4, "NameShouldBeManifest");

        // [i] Manifest Hints & Reminders
        ManifestHint MGeneralHint;
        ManifestHint MPlaceholderReminder;
        ManifestHint MWrongCall;
        ManifestHint MNotNeeded;
        ManifestHint MUsingZipfile;


        public ManifestHelper(string manifest, bool ignoreHints, string[] placeholders)
        {

            file = manifest;
            this.ignoreHints = ignoreHints;
            this.placeholders = placeholders;

            ///////////////////////////////////////
            
            // [i] Manifest Hints & Reminders
            MGeneralHint = new(0, "GeneralHint", ignoreHints);
            MPlaceholderReminder = new(1, "PlaceholderTextReminder", ignoreHints, true);
            MWrongCall = new(2, "UsedExcludeInsteadOfUncompatible", ignoreHints);
            MNotNeeded = new(3, "UnnecessaryParameter", ignoreHints);
            MUsingZipfile = new(4, "UsingZipfileParameter", ignoreHints);

        }

        bool IsValidManifestFile()
        {
            return File.Exists(file) & (Path.GetExtension(file).ToLower() == ".json");
        }

        JSON LoadJsonManifest()
        {

            manifestJson = new();
            manifestJson.LoadFromFile(file);
            return manifestJson;

        }

        bool IsValidVersionName(string version)
        {

            return Regex.IsMatch(version, @"^v(1000|[1-9][0-9]{0,2})$");

        }

        void AnalyseVersion(JSON versionJson, string versionTag)
        {

            foreach (var pair in versionJson.GetKeyValuePairs())
            {

                switch (pair.Key)
                {

                    case "uncompatible":
                        if (pair.Value is string[] uncompatibleVersions)
                        {

                            if (uncompatibleVersions.Length < 1)
                            {
                                problems++;
                                MRedundantParameter.PrintMessage($"'uncompatible' was specified but is empty - {versionTag}:{pair.Key}");
                            }

                            else
                            {

                                foreach (string specifiedVersion in uncompatibleVersions)
                                {

                                    if (Regex.IsMatch(specifiedVersion, @"v[3-9]\.[0-9]{1,2}\.[0-9]+") | Regex.IsMatch(specifiedVersion, @"v10\.[0-6]\.[0-2]"))
                                    {
                                        problems++;
                                        MRedundantUseOfUncompatible.PrintMessage($"{specifiedVersion} is uncompatible with MANIFEST files by default, so it can be removed from the list - {versionTag}:{pair.Key}");
                                    }
                                    else if (!Regex.IsMatch(specifiedVersion, @"v[1-9][0-9]+\.[0-9]+\.[0-9]+"))
                                    {
                                        problems++;
                                        MRedundantUseOfUncompatible.PrintMessage($"{specifiedVersion} is not a WriterClassic version - {versionTag}:{pair.Key}");
                                    }

                                }

                            }

                        }

                        else
                        {
                            problems++;
                            MTypeError.PrintMessage($"Expected an array of strings but got another data type instead - {versionTag}:{pair.Key}");
                        }
                        break;

                    case "name":
                        if (pair.Value is string pluginName)
                        {

                            foreach (char forbiddenChar in Path.GetInvalidPathChars())
                            {
                                if (pluginName.Contains(forbiddenChar))
                                {
                                    problems++;
                                    MForbiddenName.PrintMessage($"Character '{forbiddenChar}' is not allowed in the 'name' parameter - {versionTag}:{pair.Key}");
                                }
                                
                            }

                            if (pluginName.Length > 20)
                            {
                                problems++;
                                MLongParameter.PrintMessage($"The string exceeds 20 characters - {versionTag}:{pair.Key}");
                            }

                        }

                        else
                        {
                            problems++;
                            MTypeError.PrintMessage($"Expected a string but got another data type instead - {versionTag}:{pair.Key}");
                        }
                        
                        break;

                    case "description":
                        if (pair.Value is string pluginDescription)
                        {
                            if (pluginDescription.Length > 60) { problems++; MLongParameter.PrintMessage($"The string exceeds 60 characters - {versionTag}:{pair.Key}"); }
                        }

                        else
                        {
                            problems++;
                            MTypeError.PrintMessage($"Expected a string but got another data type instead - {versionTag}:{pair.Key}");
                        }
                        
                        break;

                    case "author":
                        if (pair.Value is not string) { problems++; MTypeError.PrintMessage($"Expected a string but got another data type instead - {versionTag}:{pair.Key}"); }                       
                        break;

                    case "imagefile":
                    case "pyfile":
                        if (pair.Value is string imagePyFile)
                        {
                            if (placeholders.Contains(imagePyFile))
                            {
                                hints++;
                                MPlaceholderReminder.PrintMessage($"Use of placeholder text; don't forget to use a correct URL to the file later on - {versionTag}:{pair.Key}");
                            }
                        }

                        else
                        {
                            problems++;
                            MTypeError.PrintMessage($"Expected a string but got another data type instead - {versionTag}:{pair.Key}");
                        }
                        
                        break;

                    case "zipfile":
                        if (pair.Value is string zipFile)
                        {
                            if (placeholders.Contains(zipFile)) {hints++; MPlaceholderReminder.PrintMessage($"Use of placeholder text; don't forget to use a correct URL to the file later on - {versionTag}:{pair.Key}"); }
                            
                            if (versionJson.GetLengthSafely() == 2 & versionJson.Contains("uncompatible")) {}
                            else if (versionJson.GetLengthSafely() > 1) { problems++; MRedundantParameter.PrintMessage($"Any parameters other than 'uncompatible' are ignored when 'zipfile' is in use - {versionTag}:{pair.Key}"); }
                        }

                        else
                        {
                            problems++;
                            MTypeError.PrintMessage($"Expected a string but got another data type instead - {versionTag}:{pair.Key}");
                        }
                        
                        break;

                    case "exclude":
                        hints++;
                        MWrongCall.PrintMessage("Used 'exclude' but probably meant 'uncompatible' - {versionTag}:{pair.Key}");
                        break;
                    
                    default:
                        MNotNeeded.PrintMessage($"{pair.Key} is not sued by WriterClassic and can be safely removed - {versionTag}:{pair.Key}");
                        break;

                }

            }

        }

        public void InitiateAnalysis()
        {

            foreach (var pair in manifestJson.GetKeyValuePairs())
            {

                if (!IsValidVersionName(pair.Key))
                {
                    problems++;
                    MInvalidVersionTag.PrintMessage($"Expected a string matching the regex pattern '^v([1-9][0-9]{0,2})$' or 'v1000' but got {pair.Key} instead");
                }

                if (pair.Value is JSON json)
                {
                    if (json.Contains("zipfile"))
                    {
                        hints++;
                        MUsingZipfile.PrintMessage($"The use of 'zipfile' is disencouraged - {pair.Key}");
                    }

                    else if (!json.Contains("pyfile"))
                    {
                        problems++;
                        MMissingParameter.PrintMessage($"One of 'pyfile'/'zipfile' must be defined - {pair.Key}");
                    }

                    AnalyseVersion(json, pair.Key);
                }
                else
                {
                    problems++;
                    MTypeError.PrintMessage($"Expected a JsonObject to be {pair.Key}'s value, but got another data type instead"); 
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Return Code (0 = success; 21 = the file does not exist)</returns>
        public int AnalyseFile()
        {

            if (!IsValidManifestFile()) return 21;
            LoadJsonManifest();

            // [*] Generic Analysis starts here
            if (Path.GetFileName(file) != "manifest.json")
            { problems++; MWrongName.PrintMessage("Expected the name of the MANIFEST file to be 'manifest.json'"); }

            return 0;

        }

        public int Problems
        {
            get { return problems; }
        }

        public int Hints
        {
            get { return hints; }
        }

        public bool IgnoreHints
        {
            get { return ignoreHints; }
        }

    }

}


using System;
using System.Diagnostics;
using System.IO;

namespace WPlugZ_CLI.Source
{

    public static class OSEnvironment
    {

        /// <summary>
        /// Get the username by evaluating USERPROFILE
        /// </summary>
        /// <returns>Username</returns>
        public static string GetUsername()
        {

            return Path.GetFileName(Environment.GetEnvironmentVariable("USERPROFILE"));

        }

        /// <summary>
        /// Deletes a directory, empty or not.
        /// </summary>
        /// <param name="directory">The directory to delete (preferably, an absolute path)</param>
        /// <returns>Return code (0 = success; -1 = failed to start the removal; anything else = rmdir return code)</returns>
        public static int DeleteDirectory(string directory)
        {

            try
            {

                ProcessStartInfo psi = new()
                {

                    FileName = "rmdir",
                    Arguments = $@"/s /q ""{directory}""",
                    RedirectStandardOutput = true, // [i] don't output anything if successful
                    UseShellExecute = true

                };
                
                using (Process removal = Process.Start(psi))
                {

                    removal.WaitForExit();
                    return removal.ExitCode;

                }

            }
            catch (Exception)
            {

                return -1;

            }

        }

    }

}

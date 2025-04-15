using System;
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
        /// <returns>Return code (0 = success; -1 = failed to remove)</returns>
        public static int DeleteDirectory(string directory)
        {

            try
            {

                // [i] the initial plan was to use rmdir, but unlike with Linux's rm -rf
                // [i] Windows needs special permisdsions even when deleting a non-root folder
                // [i] for some fucking reason
                // [i] so fuck you Microsoft!
                Directory.Delete(directory, true);
                return 0;

            }
            catch (Exception)
            {

                return -1;

            }

        }

        public static string PythonDenominator
        {

            get { return "python"; }

        }

    }

}

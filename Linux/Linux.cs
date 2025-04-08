using System;

namespace WPlugZ_CLI.Source
{

    public static class OSEnvironment
    {

        /// <summary>
        /// Get the username by evaluating HOME
        /// </summary>
        /// <returns>Username</returns>
        public static string GetUsername()
        {

            return Environment.GetEnvironmentVariable("HOME");

        }

    }

}

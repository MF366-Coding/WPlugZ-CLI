using System;
using System.Diagnostics;


namespace WPlugZ_CLI.Source
{
    
    public static class WebBrowser
    {

        /// <summary>
        /// Opens a URL with the default webbrowser.
        /// </summary>
        /// <param name="url">The URL to open</param>
        /// <returns>Return code (0 = Success; -1 = Error)</returns>
        public static int OpenURL(string url)
        {

            try
            {

                ProcessStartInfo psi = new()
                {

                    FileName = url,
                    UseShellExecute = true

                };
                Process.Start(psi);
                return 0;

            }
            catch (Exception)
            {

                return -1;

            }

        }

    }

}

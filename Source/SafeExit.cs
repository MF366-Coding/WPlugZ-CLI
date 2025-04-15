using System;


namespace WPlugZ_CLI.Source
{

    public static class ExitAfter
    {

        static void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Exit only after a full style reset is applied.
        /// </summary>
        /// <param name="exitCode">The exit code (0 = success; anything else = error)</param>
        public static void ColorReset(int exitCode = 0)
        {

            Colors.ResetAllEffects();
            Exit(exitCode);

        }

    }

}


using System;

namespace WPlugZ_CLI.Source
{

    public static class Colors
    {

        public static readonly string BLACK = "\u001b[30m";
        public static readonly string RED = "\u001b[31m";
        public static readonly string GREEN = "\u001b[32m";
        public static readonly string YELLOW = "\u001b[33m";
        public static readonly string BLUE = "\u001b[34m";
        public static readonly string MAGENTA = "\u001b[35m";
        public static readonly string CYAN = "\u001b[36m";
        public static readonly string WHITE = "\u001b[37m";
        public static readonly string BRIGHT_BLACK = "\u001b[90m";
        public static readonly string BRIGHT_RED = "\u001b[91m";
        public static readonly string BRIGHT_GREEN = "\u001b[92m";
        public static readonly string BRIGHT_YELLOW = "\u001b[93m";
        public static readonly string BRIGHT_BLUE = "\u001b[94m";
        public static readonly string BRIGHT_MAGENTA = "\u001b[95m";
        public static readonly string BRIGHT_CYAN = "\u001b[96m";
        public static readonly string BRIGHT_WHITE = "\u001b[97m";
        public static readonly string RESET = "\u001b[39m"; // [i] resets foreground only
        public static readonly string STRONG_RESET = "\u001b[0m"; // [i] resets all styling :))

        public static string Use256ColorCode(int code)
        {
            return $"\u001b[38;5;{code}m";
        }

        public static string PickRandom256ColorCode(Random random)
        {   
            return Use256ColorCode(Numbers.ClampInteger(random.Next(), 0, 255));
        }

        public static string PickLightish256ColorCode(Random random)
        {   
            return Use256ColorCode(Numbers.ClampInteger(random.Next(), 18, 232)); // [i] enforces light-ish colors (ones that can actually be seen on dark backgrounds)
        }

        public static void ResetAllEffects()
        {   
            Console.Write(STRONG_RESET);
        }

    }

}
using System;

namespace WPlugZ_CLI.Source
{

    public static class Logger
    {

        public static void Info(string data, int tonality = 1)
        {

			string color;
            if (tonality == 0) color = Colors.CYAN; // [i] lightest
            else if (tonality == 2) color = Colors.Use256ColorCode(19); // [i] darkest
            else color = Colors.BLUE;
            Console.Write(color);
            Console.Write(data);

        }

        public static void Warning(string data, int tonality = 1)
        {

			string color;
            if (tonality == 0) color = Colors.BRIGHT_YELLOW; // [i] lightest
            else if (tonality == 2) color = Colors.Use256ColorCode(172); // [i] darkest
            else color = Colors.YELLOW;
            Console.Write(color);
            Console.Write(data);

        }

        public static void Error(string data, int tonality = 1)
        {

			string color;
            if (tonality == 0) color = Colors.BRIGHT_RED; // [i] lightest
            else if (tonality == 2) color = Colors.BRIGHT_MAGENTA; // [i] darkest
            else color = Colors.RED;
            Console.Write(color);
            Console.Write(data);

        }

        public static void Success(string data, int tonality = 1)
        {

			string color;
            if (tonality == 0) color = Colors.BRIGHT_GREEN; // [i] lightest
            else if (tonality == 2) color = Colors.Use256ColorCode(70); // [i] darkest
            else color = Colors.GREEN;
            Console.Write(color);
            Console.Write(data);

        }

        public static void Write(string data, int colorCode = 255)
        {

            Console.Write(Colors.Use256ColorCode(colorCode));
            Console.Write(data);

        }

        public static void WriteLine(string data, int colorCode = 255)
        {

            Console.Write(Colors.Use256ColorCode(colorCode));
            Console.WriteLine(data);

        }

        public static void InfoLine(string data, int tonality = 1)
        {

            Info(data, tonality);
			Console.Write("\n");

        }

        public static void WarningLine(string data, int tonality = 1)
        {

            Warning(data, tonality);
			Console.Write("\n");

        }

        public static void ErrorLine(string data, int tonality = 1)
        {

            Error(data, tonality);
			Console.Write("\n");

        }

        public static void SuccessLine(string data, int tonality = 1)
        {

            Success(data, tonality);
			Console.Write("\n");

        }

    }

}

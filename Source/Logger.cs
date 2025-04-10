using System;

namespace WPlugZ_CLI.Source
{

    public static class Logger
    {

        public static void Info(string data, int tonality = 1)
        {

            if (tonality == 0) tonality = 69; // [i] lightest
            else if (tonality == 2) tonality = 19; // [i] darkest
            else tonality = 27;
            Console.Write(Colors.Use256ColorCode(tonality));
            Console.Write(data);

        }

        public static void Warning(string data, int tonality = 1)
        {

            if (tonality == 0) tonality = 191; // [i] lightest
            else if (tonality == 2) tonality = 172; // [i] darkest
            else tonality = 184;
            Console.Write(Colors.Use256ColorCode(tonality));
            Console.Write(data);

        }

        public static void Error(string data, int tonality = 1)
        {

            if (tonality == 0) tonality = 202; // [i] lightest
            else if (tonality == 2) tonality = 88; // [i] darkest
            else tonality = 196;
            Console.Write(Colors.Use256ColorCode(tonality));
            Console.Write(data);

        }

        public static void Success(string data, int tonality = 1)
        {

            if (tonality == 0) tonality = 118; // [i] lightest
            else if (tonality == 2) tonality = 70; // [i] darkest
            else tonality = 82;
            Console.Write(Colors.Use256ColorCode(tonality));
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

            if (tonality == 0) tonality = 69; // [i] lightest
            else if (tonality == 2) tonality = 19; // [i] darkest
            else tonality = 27;
            Console.Write(Colors.Use256ColorCode(tonality));
            Console.WriteLine(data);

        }

        public static void WarningLine(string data, int tonality = 1)
        {

            if (tonality == 0) tonality = 191; // [i] lightest
            else if (tonality == 2) tonality = 172; // [i] darkest
            else tonality = 184;
            Console.Write(Colors.Use256ColorCode(tonality));
            Console.WriteLine(data);

        }

        public static void ErrorLine(string data, int tonality = 1)
        {

            if (tonality == 0) tonality = 202; // [i] lightest
            else if (tonality == 2) tonality = 88; // [i] darkest
            else tonality = 196;
            Console.Write(Colors.Use256ColorCode(tonality));
            Console.WriteLine(data);

        }

        public static void SuccessLine(string data, int tonality = 1)
        {

            if (tonality == 0) tonality = 118; // [i] lightest
            else if (tonality == 2) tonality = 70; // [i] darkest
            else tonality = 82;
            Console.Write(Colors.Use256ColorCode(tonality));
            Console.WriteLine(data);

        }

    }

}

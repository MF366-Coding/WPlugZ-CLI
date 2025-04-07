using System;
using System.CommandLine;
using System.Threading.Tasks;


namespace WPlugZ_CLI
{
    public class Commands
    {
        public static async Task CreateRootCommand(string[] args)
        {
            
            var rootCommand = new RootCommand("THE only CLI you'll need for managing WriterClassic plugins");

            rootCommand.SetHandler(() => {

                Console.Write("\nNow written in C#\nMF366 * 2025 * MIT License\n\nHello, World!\n");

            });

            await rootCommand.InvokeAsync(args);

        }
    }
}

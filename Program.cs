using System;
using System.CommandLine;
using System.Threading.Tasks;
using WPlugZ_CLI;


class Program
{

    static async Task Main(string[] args)
    {

        await Commands.CreateRootCommand(args);

    }

}


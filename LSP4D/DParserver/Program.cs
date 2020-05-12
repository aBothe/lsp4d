using System;
using System.Threading.Tasks;

namespace D_Parserver
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var server = await DLanguageServerFactory
                .CreateServer(Console.OpenStandardInput(), Console.OpenStandardOutput());

            await server.WaitForExit;
        }
    }
}
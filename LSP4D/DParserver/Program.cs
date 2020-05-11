using System;
using AustinHarris.JsonRpc;

namespace D_Parserver
{
    class Program
    {
        static object[] _services;

        static Program()
        {
            _services = new object[] {
                new DParserLspService()
            };
        }

        static void Main(string[] args)
        {
            var rpcResultHandler = new AsyncCallback(_ => Console.WriteLine(((JsonRpcStateAsync)_).Result));

            for (string line = Console.ReadLine(); !string.IsNullOrEmpty(line); line = Console.ReadLine())
            {
                JsonRpcProcessor.Process(new JsonRpcStateAsync(rpcResultHandler, null) {JsonRpc = line});
            }
        }
    }
}
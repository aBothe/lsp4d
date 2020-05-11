using System;
using AustinHarris.JsonRpc;
using D_Parserver.Lsp;

namespace D_Parserver
{
    public class DParserLspService : JsonRpcService, ILspService
    {
        [JsonRpcMethod("$/cancelRequest")]
        public void CancelRequest(string requestId)
        {
        }

        [JsonRpcMethod("$/progress")]
        public void NotifyProgress(string progressToken, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
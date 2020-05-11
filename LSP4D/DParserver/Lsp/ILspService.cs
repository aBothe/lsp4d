using System;

namespace D_Parserver.Lsp
{
    public interface ILspService
    {
        void CancelRequest(string requestId);

        void NotifyProgress(string progressToken, object value);
    }
}
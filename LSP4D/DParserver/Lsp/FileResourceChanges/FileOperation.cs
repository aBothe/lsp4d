using D_Parserver.Lsp.BasicStructures;

namespace D_Parserver.Lsp.FileResourceChanges
{
    public class FileOperation : IDocumentOperation
    {
        public readonly string kind;

        public FileOperation(string kind)
        {
            this.kind = kind;
        }
    }
}
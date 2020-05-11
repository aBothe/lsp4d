using D_Parserver.Lsp.FileResourceChanges;

namespace D_Parserver.Lsp.BasicStructures
{
    public class TextDocumentEdit : IDocumentOperation
    {
        /// <summary>
        /// The text document to change.
        /// </summary>
        public VersionedTextDocumentIdentifier textDocument;

        /// <summary>
        /// The edits to be applied.
        /// </summary>
        public TextEdit[] edits;
    }
}
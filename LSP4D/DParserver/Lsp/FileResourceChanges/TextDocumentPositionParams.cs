using D_Parserver.Lsp.BasicStructures;

namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocumentPositionParams
    /// </summary>
    public class TextDocumentPositionParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier textDocument;

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        public Position position;
    }
}
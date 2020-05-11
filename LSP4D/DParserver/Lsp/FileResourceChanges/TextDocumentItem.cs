using System;

namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocumentItem
    /// </summary>
    public class TextDocumentItem
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        public Uri uri;

        /// <summary>
        /// The text document's language identifier.
        /// </summary>
        public string languageId;

        /// <summary>
        /// The version number of this document (it will increase after each
        /// change, including undo/redo).
        /// </summary>
        public long version;

        /// <summary>
        /// The content of the opened text document.
        /// </summary>
        public string text;
    }
}
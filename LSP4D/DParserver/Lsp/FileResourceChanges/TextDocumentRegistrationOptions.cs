namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocumentRegistrationOptions
    /// </summary>
    public struct TextDocumentRegistrationOptions
    {
        /// <summary>
        /// A document selector to identify the scope of the registration. If set to null
        /// the document selector provided on the client side will be used.
        /// </summary>
        public DocumentFilter[] documentSelector;
    }
}
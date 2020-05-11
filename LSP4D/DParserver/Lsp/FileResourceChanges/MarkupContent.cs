namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#markupContent
    /// </summary>
    public struct MarkupContent
    {
        /// <summary>
        /// The type of the Markup
        /// </summary>
        public MarkupKind kind;

        /// <summary>
        /// The content itself
        /// </summary>
        public string value;
    }
}
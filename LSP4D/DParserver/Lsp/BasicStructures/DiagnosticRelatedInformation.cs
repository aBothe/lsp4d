namespace D_Parserver.Lsp.BasicStructures
{
    /// <summary>
    /**
     * Represents a related message and source code location for a diagnostic. This should be
     * used to point to code locations that cause or are related to a diagnostics, e.g when duplicating
     * a symbol in a scope.
     */
    /// </summary>
    public struct DiagnosticRelatedInformation
    {
        /// <summary>
        /// The location of this related diagnostic information.
        /// </summary>
        public Location location;

        /// <summary>
        /// The message of this related diagnostic information.
        /// </summary>
        public string message;
    }
}
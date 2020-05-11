namespace D_Parserver.Lsp.BasicStructures
{
    public class Diagnostic
    {
        /// <summary>
        /// The range at which the message applies.
        /// </summary>
        public Range range;

        /// <summary>
        /**
	     * The diagnostic's severity. Can be omitted. If omitted it is up to the
	     * client to interpret diagnostics as error, warning, info or hint.
	     */
        /// </summary>
        public DiagnosticSeverity severity;

        /// <summary>
        /// The diagnostic's code, which might appear in the user interface.
        /// </summary>
        public string code;

        /** <summary>
		 * A human-readable string describing the source of this
		 * diagnostic, e.g. 'typescript' or 'super lint'.</summary>
		 */
        public string source;

        /** <summary>
		 * The diagnostic's message.
         * </summary>
		 */
        public string message;

        /** <summary>
		 * Additional metadata about the diagnostic.
		 * </summary>
         * <remarks>since 3.15.0</remarks>
		 */
        public DiagnosticTag[] tags;

        /** <summary>
		 * An array of related diagnostic information, e.g. when symbol-names within
		 * a scope collide all definitions can be marked via this property.
         * </summary>
		 */
        public DiagnosticRelatedInformation[] relatedInformation;
    }
}
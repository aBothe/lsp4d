namespace D_Parserver.Lsp
{
    public enum DiagnosticTag : int
    {
        /// <summary>
        /**
         * Unused or unnecessary code.
         *
         * Clients are allowed to render diagnostics with this tag faded out instead of having
         * an error squiggle.
         */
        /// </summary>
        Unnecessary = 1,

        /// <summary>
        /**
         * Deprecated or obsolete code.
         *
         * Clients are allowed to rendered diagnostics with this tag strike through.
         */
        /// </summary>
        Deprecated = 2
    }
}
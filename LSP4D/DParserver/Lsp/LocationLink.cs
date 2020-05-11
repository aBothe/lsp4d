using System;

namespace D_Parserver.Lsp
{
    public struct LocationLink
    {
        /// <summary>
        /**
        * Span of the origin of this link.
        *
        * Used as the underlined span for mouse interaction. Defaults to the word range at
        * the mouse position.
        */
        /// </summary>
        public Range? originSelectionRange;

        /// <summary>
        /// The target resource identifier of this link.
        /// </summary>
        public Uri targetUri;

        /// <summary>
        /**
		 * The full target range of this link. If the target for example is a symbol then target range is the
		 * range enclosing this symbol not including leading/trailing whitespace but everything else
		 * like comments. This information is typically used to highlight the range in the editor.
		 */
        /// </summary>
        public Range targetRange;

        /// <summary>
        /**
		 * The range that should be selected and revealed when this link is being followed, e.g the name of a function.
		 * Must be contained by the the `targetRange`. See also `DocumentSymbol#range`
		 */
        /// </summary>
        public Range targetSelectionRange;
    }
}
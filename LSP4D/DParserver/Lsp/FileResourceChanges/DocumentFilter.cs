namespace D_Parserver.Lsp.FileResourceChanges
{
	/// <summary>
	/// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#documentFilter
	/// </summary>
    public struct DocumentFilter
    {
        /// <summary>
        /// A language id, like `typescript`.
        /// </summary>
        public string language;

        /// <summary>
        /// A Uri [scheme](#Uri.scheme), like `file` or `untitled`.
        /// </summary>
        public string scheme;

        /// <summary>
        /**
		 * A glob pattern, like `*.{ts,js}`.
		 *
		 * Glob patterns can have the following syntax:
		 * - `*` to match one or more characters in a path segment
		 * - `?` to match on one character in a path segment
		 * - `**` to match any number of path segments, including none
		 * - `{}` to group conditions (e.g. `**​/*.{ts,js}` matches all TypeScript and JavaScript files)
		 * - `[]` to declare a range of characters to match in a path segment (e.g., `example.[0-9]` to match on `example.0`, `example.1`, …)
		 * - `[!...]` to negate a range of characters to match in a path segment (e.g., `example.[!0-9]` to match on `example.a`, `example.b`, but not `example.0`)
		 */
        /// </summary>
        public string pattern;
    }
}
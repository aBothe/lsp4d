namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#workDoneProgressBegin
    /// </summary>
    public class WorkDoneProgressBegin : WorkDoneProgress
    {
        /** <summary>
		 * Mandatory title of the progress operation. Used to briefly inform about
		 * the kind of operation being performed.
		 *
		 * Examples: "Indexing" or "Linking dependencies".</summary>
		 */
        public string title;

        /** <summary>
		 * Controls if a cancel button should show to allow the user to cancel the
		 * long running operation. Clients that don't support cancellation are allowed
		 * to ignore the setting.</summary>
		 */
        public bool? cancellable;

        /** <summary>
		 * Optional, more detailed associated progress message. Contains
		 * complementary information to the `title`.
		 *
		 * Examples: "3/25 files", "project/src/module2", "node_modules/some_dep".
		 * If unset, the previous progress message (if any) is still valid.</summary>
		 */
        public string message;

        /** <summary>
		 * Optional progress percentage to display (value 100 is considered 100%).
		 * If not provided infinite progress is assumed and clients are allowed
		 * to ignore the `percentage` value in subsequent in report notifications.
		 *
		 * The value should be steadily rising. Clients are free to ignore values
		 * that are not following this rule.</summary>
		 */
        public long? percentage;

	    public WorkDoneProgressBegin() : base("begin")
	    {
	    }
	}
}
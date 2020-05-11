namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#workDoneProgressEnd
    /// </summary>
    public class WorkDoneProgressEnd : WorkDoneProgress
    {
	    /** <summary>
		 * Optional, a final message indicating to for example indicate the outcome
		 * of the operation.</summary>
		 */
        public string message;

        public WorkDoneProgressEnd() : base("end")
	    {
	    }
	}
}
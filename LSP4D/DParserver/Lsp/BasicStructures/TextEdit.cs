namespace D_Parserver.Lsp.BasicStructures
{
    public class TextEdit
    {
        /// <summary>
        /**
         * The range of the text document to be manipulated. To insert
         * text into a document create a range where start === end.
         */
        /// </summary>
        public Range range;

        /// <summary>
        /**
	     * The string to be inserted. For delete operations use an
	     * empty string.
	     */
        /// </summary>
        public string newText;
    }
}
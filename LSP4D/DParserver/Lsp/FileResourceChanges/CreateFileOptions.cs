namespace D_Parserver.Lsp.FileResourceChanges
{
    public struct CreateFileOptions
    {
        /// <summary>
        /// Overwrite existing file. Overwrite wins over `ignoreIfExists`
        /// </summary>
        public bool? overwrite;

        /// <summary>
        /// Ignore if exists.
        /// </summary>
        public bool? ignoreIfExists;
    }
}
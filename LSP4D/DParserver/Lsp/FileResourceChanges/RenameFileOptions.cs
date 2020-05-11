namespace D_Parserver.Lsp.FileResourceChanges
{
    public struct RenameFileOptions
    {
        /// <summary>
        /// Overwrite target if existing. Overwrite wins over `ignoreIfExists`
        /// </summary>
        public bool? overwrite;

        /// <summary>
        /// Ignores if target exists.
        /// </summary>
        public bool? ignoreIfExists;
    }
}
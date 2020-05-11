namespace D_Parserver.Lsp.FileResourceChanges
{
    public struct DeleteFileOptions
    {
        /// <summary>
        /// Delete the content recursively if a folder is denoted.
        /// </summary>
        public bool? recursive;

        /// <summary>
        /// Ignore the operation if the file doesn't exist.
        /// </summary>
        public bool? ignoreIfNotExists;
    }
}
using System;

namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// Delete file operation
    /// </summary>
    public class DeleteFile : FileOperation
    {
        /// <summary>
        /// The file to delete.
        /// </summary>
        public Uri uri;

        public DeleteFileOptions? options;

        public DeleteFile() : base("delete")
        {
        }
    }
}
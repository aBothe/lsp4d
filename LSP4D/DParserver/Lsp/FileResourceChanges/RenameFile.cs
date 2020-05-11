using System;

namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// Rename file operation
    /// </summary>
    public class RenameFile : FileOperation
    {
        /// <summary>
        /// The old (existing) location.
        /// </summary>
        public Uri oldUri;
        
        /// <summary>
        /// The new location.
        /// </summary>
        public Uri newUri;

        public RenameFileOptions? options;

        public RenameFile() : base("rename")
        {
        }
    }
}
using System;

namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// Create file operation
    /// </summary>
    public class CreateFile : FileOperation
    {
        /// <summary>
        /// The resource to create.
        /// </summary>
        public Uri uri;

        public CreateFileOptions? options;

        public CreateFile() : base("create")
        {
        }
    }
}
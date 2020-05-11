namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#workspaceEditClientCapabilities
    /// </summary>
    public class WorkspaceEditClientCapabilities
    {
        /// <summary>
        /// The client supports versioned document changes in `WorkspaceEdit`s
        /// </summary>
        public bool? documentChanges;

        /// <summary>
        /// The resource operations the client supports. Clients should at least
        /// support 'create', 'rename' and 'delete' files and folders.
        /// </summary>
        /// <remarks>@since 3.13.0</remarks>
        public ResourceOperationKind[] resourceOperations;

        /// <summary>
        /// The failure handling strategy of a client if applying the workspace edit
        /// fails.
        /// </summary>
        /// <remarks>@since 3.13.0</remarks>
        public FailureHandlingKind failureHandling;
    }
}
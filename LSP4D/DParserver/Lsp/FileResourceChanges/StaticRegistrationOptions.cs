namespace D_Parserver.Lsp.FileResourceChanges
{
    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/specification-current/#staticRegistrationOptions
    /// </summary>
    public struct StaticRegistrationOptions
    {
        /// <summary>
        /// The id used to register the request. The id can be used to deregister
        /// the request again. See also Registration#id.
        /// </summary>
        public string id;
    }
}
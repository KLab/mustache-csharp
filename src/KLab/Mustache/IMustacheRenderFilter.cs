namespace KLab.Mustache
{
    /// <summary>
    /// Allows control over whether a section is rendered or not.
    /// </summary>
    public interface IMustacheRenderFilter
    {
        /// <summary>
        /// Should return <see langword="true"/> if matching section is to rendered; <see langword="false"/> otherwise.
        /// </summary>
        bool ShouldRender { get; }
    }
}

#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Interface implemented by layouts and layout renderers.
    /// </summary>
    internal interface IRenderable
    {
        /// <summary>
        ///     Renders the the value of layout or layout renderer in the context of the specified log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>String representation of a layout.</returns>
        string Render(LogEventInfo logEvent);
    }
}
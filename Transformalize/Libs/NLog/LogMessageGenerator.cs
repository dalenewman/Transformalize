#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Returns a log message. Used to defer calculation of
    ///     the log message until it's actually needed.
    /// </summary>
    /// <returns>Log message.</returns>
    public delegate string LogMessageGenerator();
}
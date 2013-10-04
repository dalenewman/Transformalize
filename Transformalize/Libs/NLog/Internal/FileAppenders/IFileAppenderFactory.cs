#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
    /// <summary>
    ///     Interface implemented by all factories capable of creating file appenders.
    /// </summary>
    internal interface IFileAppenderFactory
    {
        /// <summary>
        ///     Opens the appender for given file name and parameters.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="parameters">Creation parameters.</param>
        /// <returns>
        ///     Instance of <see cref="BaseFileAppender" /> which can be used to write to the file.
        /// </returns>
        BaseFileAppender Open(string fileName, ICreateFileParameters parameters);
    }
}
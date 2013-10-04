#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Web service protocol.
    /// </summary>
    public enum WebServiceProtocol
    {
        /// <summary>
        ///     Use SOAP 1.1 Protocol.
        /// </summary>
        Soap11,

        /// <summary>
        ///     Use SOAP 1.2 Protocol.
        /// </summary>
        Soap12,

        /// <summary>
        ///     Use HTTP POST Protocol.
        /// </summary>
        HttpPost,

        /// <summary>
        ///     Use HTTP GET Protocol.
        /// </summary>
        HttpGet,
    }
}
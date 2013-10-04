#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.LogReceiverService
{
#if WCF_SUPPORTED && !SILVERLIGHT && !NET_CF

namespace NLog.LogReceiverService
{
    using System.ServiceModel;

    /// <summary>
    /// Service contract for Log Receiver server.
    /// </summary>
    [ServiceContract(Namespace = LogReceiverServiceConfig.WebServiceNamespace)]
    public interface ILogReceiverServer
    {
        /// <summary>
        /// Processes the log messages.
        /// </summary>
        /// <param name="events">The events.</param>
        [OperationContract]
        void ProcessLogMessages(NLogEvents events);
    }
}

#endif
}
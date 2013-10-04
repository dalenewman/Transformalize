#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.LogReceiverService
{
#if WCF_SUPPORTED
    using System.ServiceModel;
#endif

    /// <summary>
    ///     Service contract for Log Receiver client.
    /// </summary>
#if WCF_SUPPORTED
    [ServiceContract(Namespace = LogReceiverServiceConfig.WebServiceNamespace, ConfigurationName = "NLog.LogReceiverService.ILogReceiverClient")]
#endif
    public interface ILogReceiverClient
    {
        /// <summary>
        ///     Begins processing of log messages.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="asyncState">Asynchronous state.</param>
        /// <returns>
        ///     IAsyncResult value which can be passed to <see cref="EndProcessLogMessages" />.
        /// </returns>
#if WCF_SUPPORTED
        [OperationContractAttribute(AsyncPattern = true, Action = "http://nlog-project.org/ws/ILogReceiverServer/ProcessLogMessages", ReplyAction = "http://nlog-project.org/ws/ILogReceiverServer/ProcessLogMessagesResponse")]
#endif
        IAsyncResult BeginProcessLogMessages(NLogEvents events, AsyncCallback callback, object asyncState);

        /// <summary>
        ///     Ends asynchronous processing of log messages.
        /// </summary>
        /// <param name="result">The result.</param>
        void EndProcessLogMessages(IAsyncResult result);
    }
}
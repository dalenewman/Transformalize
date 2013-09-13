#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
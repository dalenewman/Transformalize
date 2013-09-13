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

using System.Diagnostics.CodeAnalysis;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Logon type.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Valid security logon type is required.")]
    public enum SecurityLogOnType
    {
        /// <summary>
        ///     Interactive Logon.
        /// </summary>
        /// <remarks>
        ///     This logon type is intended for users who will be interactively using the computer, such as a user being logged on
        ///     by a terminal server, remote shell, or similar process.
        ///     This logon type has the additional expense of caching logon information for disconnected operations;
        ///     therefore, it is inappropriate for some client/server applications,
        ///     such as a mail server.
        /// </remarks>
        Interactive = 2,

        /// <summary>
        ///     Network Logon.
        /// </summary>
        /// <remarks>
        ///     This logon type is intended for high performance servers to authenticate plaintext passwords.
        ///     The LogonUser function does not cache credentials for this logon type.
        /// </remarks>
        Network = 3,

        /// <summary>
        ///     Batch Logon.
        /// </summary>
        /// <remarks>
        ///     This logon type is intended for batch servers, where processes may be executing on behalf of a user without
        ///     their direct intervention. This type is also for higher performance servers that process many plaintext
        ///     authentication attempts at a time, such as mail or Web servers.
        ///     The LogonUser function does not cache credentials for this logon type.
        /// </remarks>
        Batch = 4,

        /// <summary>
        ///     Logon as a Service.
        /// </summary>
        /// <remarks>
        ///     Indicates a service-type logon. The account provided must have the service privilege enabled.
        /// </remarks>
        Service = 5,

        /// <summary>
        ///     Network Clear Text Logon.
        /// </summary>
        /// <remarks>
        ///     This logon type preserves the name and password in the authentication package, which allows the server to make
        ///     connections to other network servers while impersonating the client. A server can accept plaintext credentials
        ///     from a client, call LogonUser, verify that the user can access the system across the network, and still
        ///     communicate with other servers.
        ///     NOTE: Windows NT:  This value is not supported.
        /// </remarks>
        NetworkClearText = 8,

        /// <summary>
        ///     New Network Credentials.
        /// </summary>
        /// <remarks>
        ///     This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
        ///     The new logon session has the same local identifier but uses different credentials for other network connections.
        ///     NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
        ///     NOTE: Windows NT:  This value is not supported.
        /// </remarks>
        NewCredentials = 9,
    }
}

#endif
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

using System.Net.Mail;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Supports mocking of SMTP Client code.
    /// </summary>
    internal class MySmtpClient : SmtpClient, ISmtpClient
    {
#if NET3_5 || NET2_0 || NETCF2_0 || NETCF3_5 || MONO
    /// <summary>
    /// Sends a QUIT message to the SMTP server, gracefully ends the TCP connection, and releases all resources used by the current instance of the <see cref="T:System.Net.Mail.SmtpClient"/> class.
    /// </summary>
        public void Dispose()
        {
            // dispose was added in .NET Framework 4.0, previous frameworks don't need it but adding it here to make the 
            // user experience the same across all
        }
#endif
    }
}

#endif
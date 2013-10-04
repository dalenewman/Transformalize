#region License
// /*
// See license included in this library folder.
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
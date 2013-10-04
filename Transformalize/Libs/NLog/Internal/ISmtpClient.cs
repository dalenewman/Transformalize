#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Net;
using System.Net.Mail;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Supports mocking of SMTP Client code.
    /// </summary>
    internal interface ISmtpClient : IDisposable
    {
        string Host { get; set; }

        int Port { get; set; }

        ICredentialsByHost Credentials { get; set; }

        bool EnableSsl { get; set; }

        void Send(MailMessage msg);
    }
}

#endif
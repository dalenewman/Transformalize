// 
// Copyright (c) 2004-2011 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using System.Text;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.Layouts;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Sends log messages by email using SMTP protocol.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Mail_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Mail/Simple/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Mail/Simple/Example.cs" />
    ///     <p>
    ///         Mail target works best when used with BufferingWrapper target
    ///         which lets you send multiple log messages in single mail
    ///     </p>
    ///     <p>
    ///         To set up the buffered mail target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Mail/Buffered/NLog.config" />
    ///     <p>
    ///         To set up the buffered mail target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Mail/Buffered/Example.cs" />
    /// </example>
    [Target("Mail")]
    public class MailTarget : TargetWithLayoutHeaderAndFooter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MailTarget" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This one is safe.")]
        public MailTarget()
        {
            Body = "${message}${newline}";
            Subject = "Message from NLog on ${machinename}";
            Encoding = Encoding.UTF8;
            SmtpPort = 25;
            SmtpAuthentication = SmtpAuthenticationMode.None;
        }

        /// <summary>
        ///     Gets or sets sender's email address (e.g. joe@domain.com).
        /// </summary>
        /// <docgen category='Message Options' order='10' />
        [RequiredParameter]
        public Layout From { get; set; }

        /// <summary>
        ///     Gets or sets recipients' email addresses separated by semicolons (e.g. john@domain.com;jane@domain.com).
        /// </summary>
        /// <docgen category='Message Options' order='11' />
        [RequiredParameter]
        public Layout To { get; set; }

        /// <summary>
        ///     Gets or sets CC email addresses separated by semicolons (e.g. john@domain.com;jane@domain.com).
        /// </summary>
        /// <docgen category='Message Options' order='12' />
        public Layout CC { get; set; }

        /// <summary>
        ///     Gets or sets BCC email addresses separated by semicolons (e.g. john@domain.com;jane@domain.com).
        /// </summary>
        /// <docgen category='Message Options' order='13' />
        public Layout Bcc { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to add new lines between log entries.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if new lines should be added; otherwise, <c>false</c>.
        /// </value>
        /// <docgen category='Layout Options' order='99' />
        public bool AddNewLines { get; set; }

        /// <summary>
        ///     Gets or sets the mail subject.
        /// </summary>
        /// <docgen category='Message Options' order='5' />
        [DefaultValue("Message from NLog on ${machinename}")]
        [RequiredParameter]
        public Layout Subject { get; set; }

        /// <summary>
        ///     Gets or sets mail message body (repeated for each log message send in one mail).
        /// </summary>
        /// <remarks>
        ///     Alias for the <c>Layout</c> property.
        /// </remarks>
        /// <docgen category='Message Options' order='6' />
        [DefaultValue("${message}")]
        public Layout Body
        {
            get { return Layout; }
            set { Layout = value; }
        }

        /// <summary>
        ///     Gets or sets encoding to be used for sending e-mail.
        /// </summary>
        /// <docgen category='Layout Options' order='20' />
        [DefaultValue("UTF8")]
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to send message as HTML instead of plain text.
        /// </summary>
        /// <docgen category='Layout Options' order='11' />
        [DefaultValue(false)]
        public bool Html { get; set; }

        /// <summary>
        ///     Gets or sets SMTP Server to be used for sending.
        /// </summary>
        /// <docgen category='SMTP Options' order='10' />
        [RequiredParameter]
        public Layout SmtpServer { get; set; }

        /// <summary>
        ///     Gets or sets SMTP Authentication mode.
        /// </summary>
        /// <docgen category='SMTP Options' order='11' />
        [DefaultValue("None")]
        public SmtpAuthenticationMode SmtpAuthentication { get; set; }

        /// <summary>
        ///     Gets or sets the username used to connect to SMTP server (used when SmtpAuthentication is set to "basic").
        /// </summary>
        /// <docgen category='SMTP Options' order='12' />
        public Layout SmtpUserName { get; set; }

        /// <summary>
        ///     Gets or sets the password used to authenticate against SMTP server (used when SmtpAuthentication is set to "basic").
        /// </summary>
        /// <docgen category='SMTP Options' order='13' />
        public Layout SmtpPassword { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether SSL (secure sockets layer) should be used when communicating with SMTP server.
        /// </summary>
        /// <docgen category='SMTP Options' order='14' />
        [DefaultValue(false)]
        public bool EnableSsl { get; set; }

        /// <summary>
        ///     Gets or sets the port number that SMTP Server is listening on.
        /// </summary>
        /// <docgen category='SMTP Options' order='15' />
        [DefaultValue(25)]
        public int SmtpPort { get; set; }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is a factory method.")]
        internal virtual ISmtpClient CreateSmtpClient()
        {
            return new MySmtpClient();
        }

        /// <summary>
        ///     Renders the logging event message and adds it to the internal ArrayList of log messages.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(new[] {logEvent});
        }

        /// <summary>
        ///     Renders an array logging events.
        /// </summary>
        /// <param name="logEvents">Array of logging events.</param>
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (var bucket in logEvents.BucketSort(c => GetSmtpSettingsKey(c.LogEvent)))
            {
                List<AsyncLogEventInfo> eventInfos = bucket.Value;
                ProcessSingleMailMessage(eventInfos);
            }
        }

        private void ProcessSingleMailMessage(List<AsyncLogEventInfo> events)
        {
            try
            {
                LogEventInfo firstEvent = events[0].LogEvent;
                LogEventInfo lastEvent = events[events.Count - 1].LogEvent;

                // unbuffered case, create a local buffer, append header, body and footer
                var bodyBuffer = new StringBuilder();
                if (Header != null)
                {
                    bodyBuffer.Append(Header.Render(firstEvent));
                    if (AddNewLines)
                    {
                        bodyBuffer.Append("\n");
                    }
                }

                foreach (AsyncLogEventInfo eventInfo in events)
                {
                    bodyBuffer.Append(Layout.Render(eventInfo.LogEvent));
                    if (AddNewLines)
                    {
                        bodyBuffer.Append("\n");
                    }
                }

                if (Footer != null)
                {
                    bodyBuffer.Append(Footer.Render(lastEvent));
                    if (AddNewLines)
                    {
                        bodyBuffer.Append("\n");
                    }
                }

                using (var msg = new MailMessage())
                {
                    SetupMailMessage(msg, lastEvent);
                    msg.Body = bodyBuffer.ToString();

                    using (ISmtpClient client = CreateSmtpClient())
                    {
                        client.Host = SmtpServer.Render(lastEvent);
                        client.Port = SmtpPort;
                        client.EnableSsl = EnableSsl;

                        InternalLogger.Debug("Sending mail to {0} using {1}:{2} (ssl={3})", msg.To, client.Host, client.Port, client.EnableSsl);
                        InternalLogger.Trace("  Subject: '{0}'", msg.Subject);
                        InternalLogger.Trace("  From: '{0}'", msg.From.ToString());
                        if (SmtpAuthentication == SmtpAuthenticationMode.Ntlm)
                        {
                            InternalLogger.Trace("  Using NTLM authentication.");
                            client.Credentials = CredentialCache.DefaultNetworkCredentials;
                        }
                        else if (SmtpAuthentication == SmtpAuthenticationMode.Basic)
                        {
                            string username = SmtpUserName.Render(lastEvent);
                            string password = SmtpPassword.Render(lastEvent);

                            InternalLogger.Trace("  Using basic authentication: Username='{0}' Password='{1}'", username, new string('*', password.Length));
                            client.Credentials = new NetworkCredential(username, password);
                        }

                        client.Send(msg);

                        foreach (AsyncLogEventInfo ev in events)
                        {
                            ev.Continuation(null);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                foreach (AsyncLogEventInfo ev in events)
                {
                    ev.Continuation(exception);
                }
            }
        }

        private string GetSmtpSettingsKey(LogEventInfo logEvent)
        {
            var sb = new StringBuilder();

            sb.Append(From.Render(logEvent));

            sb.Append("|");
            sb.Append(To.Render(logEvent));

            sb.Append("|");
            if (CC != null)
            {
                sb.Append(CC.Render(logEvent));
            }

            sb.Append("|");
            if (Bcc != null)
            {
                sb.Append(Bcc.Render(logEvent));
            }

            sb.Append("|");
            sb.Append(SmtpServer.Render(logEvent));
            if (SmtpPassword != null)
            {
                sb.Append(SmtpPassword.Render(logEvent));
            }

            sb.Append("|");
            if (SmtpUserName != null)
            {
                sb.Append(SmtpUserName.Render(logEvent));
            }

            return sb.ToString();
        }

        private void SetupMailMessage(MailMessage msg, LogEventInfo logEvent)
        {
            msg.From = new MailAddress(From.Render(logEvent));
            foreach (string mail in To.Render(logEvent).Split(';'))
            {
                msg.To.Add(mail);
            }

            if (Bcc != null)
            {
                foreach (string mail in Bcc.Render(logEvent).Split(';'))
                {
                    msg.Bcc.Add(mail);
                }
            }

            if (CC != null)
            {
                foreach (string mail in CC.Render(logEvent).Split(';'))
                {
                    msg.CC.Add(mail);
                }
            }

            msg.Subject = Subject.Render(logEvent);
            msg.BodyEncoding = Encoding;
            msg.IsBodyHtml = Html;
            msg.Priority = MailPriority.Normal;
        }
    }
}

#endif
using System;
using System.IO;
using System.Net.Mail;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Libs.SemanticLogging.Formatters;
using Transformalize.Main;
using Transformalize.Main.Providers.Mail;

namespace Transformalize.Logging {

    public sealed class EmailSink : IObserver<EventEntry> {
        private readonly Log _log;
        private readonly MailConnection _mail;
        private readonly IEventTextFormatter _formatter = new LegacyLogFormatter();

        public EmailSink(Log log) {
            _log = log;
            _mail = (MailConnection)log.Connection;
        }

        public void OnNext(EventEntry entry) {
            if (entry == null)
                return;
            using (var writer = new StringWriter()) {
                _formatter.WriteEvent(entry, writer);
                if (_log.Async) {
                    SendAsync(writer.ToString());
                } else {
                    Send(writer.ToString());
                }
            }
        }

        private async void SendAsync(string body) {

            using (var client = _mail.SmtpClient)
            using (var message = new MailMessage() {
                From = new MailAddress(_log.From),
                Body = body,
                Subject = _log.Subject
            }) {
                foreach (var to in _log.To.Split(',')) {
                    message.To.Add(new MailAddress(to));
                }

                try {
                    await client.SendMailAsync(message).ConfigureAwait(false);
                } catch (SmtpException e) {
                    SemanticLoggingEventSource.Log.CustomSinkUnhandledFault("SMTP error sending email: " + e.Message);
                } catch (InvalidOperationException e) {
                    SemanticLoggingEventSource.Log.CustomSinkUnhandledFault("Configuration error sending email: " + e.Message);
                }
            }
        }

        private void Send(string body) {

            using (var client = _mail.SmtpClient)
            using (var message = new MailMessage() {
                From = new MailAddress(_log.From),
                Body = body,
                Subject = _log.Subject
            }) {
                foreach (var to in _log.To.Split(',')) {
                    message.To.Add(new MailAddress(to));
                }

                try {
                    client.Send(message);
                } catch (SmtpException e) {
                    SemanticLoggingEventSource.Log.CustomSinkUnhandledFault("SMTP error sending email: " + e.Message);
                } catch (InvalidOperationException e) {
                    SemanticLoggingEventSource.Log.CustomSinkUnhandledFault("Configuration error sending email: " + e.Message);
                }
            }
        }

        public void OnCompleted() {
        }

        public void OnError(Exception error) {
        }
    }

}

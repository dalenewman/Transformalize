using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Transformalize.Logging;
using Transformalize.Main.Providers.Mail;

namespace Transformalize.Main {

    public class TemplateActionMail : TemplateActionHandler {

        private readonly char[] _addressDelimiter = ",".ToCharArray();

        public override void Handle(TemplateAction action) {

            var isTemplate = string.IsNullOrEmpty(action.File);
            var file = isTemplate ? action.RenderedFile : action.File;

            var mail = new MailMessage {
                From = new MailAddress(action.From)
            };

            if (action.To.Equals(string.Empty)) {
                TflLogger.Warn(action.ProcessName, string.Empty, "Couldn't send email. No 'to' provided.");
                return;
            }

            if (action.Connection == null) {
                TflLogger.Warn(action.ProcessName, string.Empty, "Couldn't send email.  Mail action needs a valid connection.");
                return;
            }

            foreach (var to in action.To.Split(_addressDelimiter)) {
                mail.To.Add(new MailAddress(to));
            }

            if (!action.Cc.Equals(string.Empty)) {
                foreach (var cc in action.Cc.Split(_addressDelimiter)) {
                    mail.CC.Add(new MailAddress(cc));
                }
            }

            if (!action.Bcc.Equals(string.Empty)) {
                foreach (var bcc in action.Bcc.Split(_addressDelimiter)) {
                    mail.Bcc.Add(new MailAddress(bcc));
                }
            }

            mail.IsBodyHtml = action.Html;
            if (!string.IsNullOrEmpty(action.Body)) {
                mail.Body = action.Body;
                if (!string.IsNullOrEmpty(file)) {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Exists) {
                        mail.Attachments.Add(new Attachment(fileInfo.FullName));
                    }
                }
            } else {
                if (!string.IsNullOrEmpty(file)) {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Exists) {
                        mail.Body = File.ReadAllText(fileInfo.FullName);
                    }
                }
            }

            mail.Subject = action.Subject;
            
            try {
                var connection = ((MailConnection)action.Connection).SmtpClient;
                connection.Send(mail);
                TflLogger.Info(action.ProcessName, string.Empty, isTemplate ? "Emailed rendered content to: {0}." : "Email sent to {0}.", action.To);
            } catch (Exception e) {
                TflLogger.Warn(action.ProcessName, string.Empty, "Couldn't send mail. {0}", e.Message);
                TflLogger.Debug(action.ProcessName, string.Empty, e.StackTrace);
            }
        }
    }
}
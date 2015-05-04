using System;
using System.IO;
using System.Net.Mail;
using Transformalize.Logging;
using Transformalize.Main.Providers.Mail;

namespace Transformalize.Main {

    public class TemplateActionMail : TemplateActionHandler {
        private readonly ILogger _logger;

        public TemplateActionMail(ILogger logger)
        {
            _logger = logger;
        }

        private readonly char[] _addressDelimiter = ",".ToCharArray();

        public override void Handle(TemplateAction action) {

            var isTemplate = string.IsNullOrEmpty(action.File);
            var file = isTemplate ? action.RenderedFile : action.File;

            if (action.To.Equals(string.Empty)) {
                _logger.Warn("Couldn't send email. No 'to' provided.");
                 return;
            }

            var mail = new MailMessage {
                From = new MailAddress(string.IsNullOrEmpty(action.From) ? action.To : action.From)
            };

            if (action.Connection == null) {
                _logger.Warn("Couldn't send email.  Mail action needs a valid connection.");
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
                _logger.Info(isTemplate ? "Emailed rendered content to: {0}." : "Email sent to {0}.", action.To);
            } catch (Exception e) {
                _logger.Warn("Couldn't send mail. {0}", e.Message);
                _logger.Debug(e.StackTrace);
            }
        }
    }
}
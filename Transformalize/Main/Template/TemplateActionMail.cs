using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;

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
                Log.Warn("Couldn't send email. No 'to' provided.");
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
                var port = action.Connection.Port > 0 ? action.Connection.Port : 25;
                if (string.IsNullOrEmpty(action.Connection.User)) {
                    new SmtpClient {
                        Port = port,
                        EnableSsl = action.Connection.EnableSsl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = true,
                        Host = action.Connection.Server
                    }.Send(mail);
                } else {
                    new SmtpClient {
                        Port = port,
                        EnableSsl = action.Connection.EnableSsl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(action.Connection.User, action.Connection.Password),
                        Host = action.Connection.Server
                    }.Send(mail);
                }
                Log.Info(isTemplate ? "Emailed rendered content to: {0}." : "Email sent to {0}.", action.To);
            } catch (Exception e) {
                Log.Warn("Couldn't send mail. {0}", e.Message);
                Log.Debug(e.StackTrace);
            }
        }
    }
}
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Transformalize.Main {

    public class TemplateActionMail : TemplateActionHandler {

        private readonly char[] _addressDelimiter = ",".ToCharArray();

        public override void Handle(TemplateAction action) {

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

            //Formatted mail body
            mail.IsBodyHtml = action.Html;
            mail.Body = File.ReadAllText(action.RenderedFile);
            mail.Subject = action.Subject;

            try {
                new SmtpClient {
                    Port = action.Port,
                    EnableSsl = action.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(action.Username, action.Password),
                    Host = action.Host
                }.Send(mail);
                Log.Info("Emailed rendered content to: {0}.", action.To);
            } catch (Exception e) {
                Log.Warn("Couldn't send mail. {0}", e.Message);
                Log.Debug(e.StackTrace);
            }
        }
    }
}
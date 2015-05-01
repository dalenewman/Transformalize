using System.Collections.Generic;
using System.Net.Mail;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Providers.Mail;

namespace Transformalize.Operations.Transform {

    public class MailOperation : ShouldRunOperation {

        private readonly MailConnection _connection;
        private readonly IParameters _parameters;
        private readonly bool _hasCc;
        private readonly bool _hasBcc;
        private readonly bool _hasSubject;
        private readonly string _ccKey;
        private readonly string _bccKey;
        private readonly string _subjectKey;
        private readonly string _fromKey;
        private readonly string _toKey;
        private readonly string _bodyKey;

        public MailOperation(MailConnection connection, IParameters parameters)
            : base(string.Empty, string.Empty) {
            _connection = connection;
            _parameters = parameters;
            
            _hasCc = parameters.ContainsName("cc");
            _ccKey = _hasCc ? parameters.GetKeyByName("cc") : string.Empty;

            _hasBcc = parameters.ContainsName("bcc");
            _bccKey = _hasBcc ? parameters.GetKeyByName("bcc") : string.Empty;

            _hasSubject = parameters.ContainsName("subject");
            _subjectKey = _hasSubject ? parameters.GetKeyByName("subject") : string.Empty;

            if (!parameters.ContainsName("from")) {
                throw new TransformalizeException(ProcessName, EntityName, "Mail transform requires parameter named from.");
            }
            _fromKey = parameters.GetKeyByName("from");

            if (!parameters.ContainsName("to")) {
                throw new TransformalizeException(ProcessName, EntityName, "Mail transform requires parameter named to.");
            }
            _toKey = parameters.GetKeyByName("to");

            if (!parameters.ContainsName("body")) {
                throw new TransformalizeException(ProcessName, EntityName, "Mail transform requires parameter named body.");
            }
            _bodyKey = parameters.GetKeyByName("body");

            Name = "Mail";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var mail = new MailMessage {
                    From = new MailAddress(ResolveParameter(_parameters, _fromKey, row)),
                };

                foreach (var to in ResolveParameter(_parameters, _toKey, row).Split(',')) {
                    mail.To.Add(new MailAddress(to));
                }

                if (_hasCc) {
                    foreach (var cc in ResolveParameter(_parameters, _ccKey, row).Split(',')) {
                        mail.CC.Add(new MailAddress(cc));
                    }
                }

                if (_hasBcc) {
                    foreach (var bcc in ResolveParameter(_parameters, _bccKey, row).Split(',')) {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                }

                mail.IsBodyHtml = true;
                mail.Body = ResolveParameter(_parameters, _bodyKey, row);
                if (_hasSubject) {
                    mail.Subject = ResolveParameter(_parameters, _subjectKey, row);
                }

                if (ShouldRun(row)) {
                    _connection.SmtpClient.Send(mail);
                } else {
                    Skip();
                }
                yield return row;
            }
        }

        private static string ResolveParameter(IParameters parameters, string key, Row row) {
            return parameters[key].ValueReferencesField ? row[parameters[key].Value].ToString() : (row[key] ?? parameters[key].Value).ToString();
        }

    }
}
#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2021 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Mail {
   public class MailWriter : IWrite {
      private readonly OutputContext _context;
      private readonly Field[] _fields;
      private readonly bool _run = true;

      public MailWriter(OutputContext context) {
         _context = context;
         _fields = context.Entity.GetAllFields().Where(f => !f.System).ToArray();
         var provided = new HashSet<string>(_fields.Select(f => f.Alias.ToLower()).Distinct());
         var needed = new[] { "to", "from", "body" };

         foreach (var name in needed) {
            if (!provided.Contains(name)) {
               _context.Warn($"Mail writer needs a {name} field in {_context.Entity.Alias}.");
               _run = false;
            }
         }

      }

      public void Write(IEnumerable<IRow> rows) {

         if (!_run) {
            return;
         }

         // https://github.com/jstedfast/MailKit

         using (var client = new MailKit.Net.Smtp.SmtpClient()) {

            client.ServerCertificateValidationCallback = CertificateValidationCallback;

            var options = SecureSocketOptions.Auto;

            if (_context.Connection.StartTls) {
               options = SecureSocketOptions.StartTls;
            } else if (_context.Connection.UseSsl) {
               options = SecureSocketOptions.SslOnConnect;
            }

            client.Connect(_context.Connection.Server, _context.Connection.Port, options);

            if (_context.Connection.User != string.Empty) {
               client.Authenticate(_context.Connection.User, _context.Connection.Password);
            }

            foreach (var row in rows) {
               var message = new MimeMessage();

               foreach (var field in _fields) {
                  switch (field.Alias.ToLower()) {
                     case "to":
                        foreach (var to in GetAddresses(row, field)) {
                           message.To.Add(new MailboxAddress(to, to));
                        }
                        break;
                     case "from":
                        var from = row[field].ToString();
                        message.From.Add(new MailboxAddress(from, from));
                        break;
                     case "cc":
                        foreach (var cc in GetAddresses(row, field)) {
                           message.Cc.Add(new MailboxAddress(cc, cc));
                        }
                        break;
                     case "bcc":
                        foreach (var bcc in GetAddresses(row, field)) {
                           message.Bcc.Add(new MailboxAddress(bcc, bcc));
                        }
                        break;
                     case "subject":
                        message.Subject = row[field].ToString();
                        break;
                     case "body":
                        if (field.Raw) {
                           var builder = new BodyBuilder { HtmlBody = row[field].ToString() };
                           message.Body = builder.ToMessageBody();
                        } else {
                           message.Body = new TextPart("plain") { Text = row[field].ToString() };
                        }
                        break;
                  }
               }

               client.Send(message);
            }


            client.Disconnect(true);
         }
      }

      private IEnumerable<string> GetAddresses(IRow row, IField field) {
         var value = row[field].ToString();
         if (value == string.Empty) {
            return new string[0];
         }

         if (value.Contains(',')) {
            return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
         }
         return value.Contains(';') ? value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) : new[] { value };
      }

      /// <summary>
      /// adapted from OrchardCore
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="certificate"></param>
      /// <param name="chain"></param>
      /// <param name="sslPolicyErrors"></param>
      /// <returns></returns>
      private bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
         if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

         _context.Error(string.Concat("SMTP Server's certificate {CertificateSubject} issued by {CertificateIssuer} ",
             "with thumbprint {CertificateThumbprint} and expiration date {CertificateExpirationDate} ",
             "is considered invalid with {SslPolicyErrors} policy errors"),
             certificate.Subject, certificate.Issuer, certificate.GetCertHashString(),
             certificate.GetExpirationDateString(), sslPolicyErrors);

         if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors) && chain?.ChainStatus != null) {
            foreach (var chainStatus in chain.ChainStatus) {
               _context.Error("Status: {Status} - {StatusInformation}", chainStatus.Status, chainStatus.StatusInformation);
            }
         }

         return false;
      }

   }
}

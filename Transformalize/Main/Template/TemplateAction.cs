#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Main.Providers;

namespace Transformalize.Main {

    public class TemplateAction {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        public string Action { get; set; }
        public AbstractConnection Connection { get; set; }
        public string File { get; set; }
        public string From { get; set; }
        public string Method { get; set; }
        public string RenderedFile { get; set; }
        public string TemplateName { get; set; }
        public string To { get; set; }
        public string Url { get; set; }
        public IEnumerable<string> Modes { get; set; }
        public string Arguments { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public bool Html { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public string Host { get; set; }
        public string Body { get; set; }
        public bool Before { get; set; }
        public bool After { get; set; }
        public bool Conditional { get; set; }

        public TemplateAction() {
            Action = string.Empty;
            Modes = new List<string>();
            File = string.Empty;
            From = string.Empty;
            Method = string.Empty;
            RenderedFile = string.Empty;
            TemplateName = string.Empty;
            To = string.Empty;
            Url = string.Empty;
            Arguments = string.Empty;
            Cc = string.Empty;
            Bcc = string.Empty;
            Html = true;
            Port = 25;
            Username = string.Empty;
            Password = string.Empty;
            Subject = string.Empty;
            Host = string.Empty;
            Body = string.Empty;
            Before = false;
            After = true;
            Conditional = false;
        }

        public TemplateAction(string template, ActionConfigurationElement action, IEnumerable<string> modes) {
            Action = action.Action;
            File = action.File;
            Method = action.Method;
            Url = action.Url;
            TemplateName = template;
            From = action.From;
            Arguments = action.Arguments;
            Bcc = action.Bcc;
            Cc = action.Cc;
            EnableSsl = action.EnableSsl;
            Html = action.Html;
            Password = action.Password;
            Port = action.Port;
            Subject = action.Subject;
            To = action.To;
            Username = action.Username;
            Host = action.Host;
            Body = action.Body;
            Modes = modes;
            Before = action.Before;
            After = action.After;
            Conditional = action.Conditional;
        }

        public void Handle(string file) {

            var handlers = new Dictionary<string, TemplateActionHandler>() {
                {"copy", new TemplateActionCopy()},
                {"open", new TemplateActionOpen()},
                {"run", new TemplateActionRun()},
                {"web", new TemplateActionWeb()},
                {"exec", new TemplateActionExecute()},
                {"mail", new TemplateActionMail()}
            };

            if (handlers.ContainsKey(Action.ToLower())) {
                RenderedFile = file;
                handlers[Action.ToLower()].Handle(this);
            } else {
                _log.Warn("The {0} action is not implemented.", Action);
            }
        }
    }
}
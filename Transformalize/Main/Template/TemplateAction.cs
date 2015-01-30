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
using Transformalize.Logging;
using Transformalize.Main.Providers;

namespace Transformalize.Main {

    public class TemplateAction {
        private readonly Process _process;

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
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Before { get; set; }
        public bool After { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int Timeout { get; set; }
        public string Command { get; set; }
        public string ProcessName { get; set; }

        public TemplateAction(Process process, string template, TflAction action) {
            _process = process;
            ProcessName = process.Name;
            Action = action.Action;
            File = action.File;
            Method = action.Method;
            Url = action.Url;
            TemplateName = template;
            From = action.From;
            Arguments = action.Arguments;
            Bcc = action.Bcc;
            Cc = action.Cc;
            Html = action.Html;
            Subject = action.Subject;
            To = action.To;
            Body = action.Body;
            Modes = action.GetModes();
            Before = action.Before;
            After = action.After;
            OldValue = action.OldValue;
            NewValue = action.NewValue;
            Command = action.Command;
            Timeout = action.TimeOut;
        }

       public void Handle(string file) {

            var handlers = new Dictionary<string, TemplateActionHandler>() {
                {"copy", new TemplateActionCopy(_process)},
                {"open", new TemplateActionOpen()},
                {"run", new TemplateActionRun()},
                {"web", new TemplateActionWeb()},
                {"tfl", new TemplateActionTfl()},
                {"exec", new TemplateActionExecute()},
                {"execute", new TemplateActionExecute()},
                {"mail", new TemplateActionMail()},
                {"replace", new TemplateActionReplace()},
                {"dostounix", new TemplateActionReplace("\r\n", "\n")},
                {"unixtodos", new TemplateActionReplace("\n","\r\n")}
            };

            if (handlers.ContainsKey(Action.ToLower())) {
                RenderedFile = file;
                handlers[Action.ToLower()].Handle(this);
            } else {
                TflLogger.Warn(_process.Name, string.Empty, "The {0} action is not implemented.", Action);
            }
        }
    }
}
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
        public Dictionary<string, TemplateActionHandler> ActionHandlers { get; set; }

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

            ActionHandlers = new Dictionary<string, TemplateActionHandler>() {
                {"copy", new TemplateActionCopy(_process)},
                {"open", new TemplateActionOpen(_process.Logger)},
                {"run", new TemplateActionRun(_process.Logger)},
                {"web", new TemplateActionWeb(_process.Logger)},
                {"tfl", new TemplateActionTfl(_process.Logger)},
                {"exec", new TemplateActionExecute(_process)},
                {"execute", new TemplateActionExecute(_process)},
                {"mail", new TemplateActionMail(_process.Logger)},
                {"replace", new TemplateActionReplace(_process.Logger)},
                {"dostounix", new TemplateActionReplace("\r\n", "\n")},
                {"unixtodos", new TemplateActionReplace("\n","\r\n")}
            };
        }

       public void Handle(string file) {

            if (ActionHandlers.ContainsKey(Action)) {
                RenderedFile = file;
                ActionHandlers[Action].Handle(this);
            } else {
                _process.Logger.Warn("The {0} action is not implemented.", Action);
            }
        }
    }
}
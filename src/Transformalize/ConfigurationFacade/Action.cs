#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Collections.Generic;
using System.Linq;
using Cfg.Net;

namespace Transformalize.ConfigurationFacade {

   public class Action : CfgNode {

      [Cfg]
      public string Type { get; set; }

      [Cfg]
      public string Name { get; set; }

      [Cfg]
      public string After { get; set; }

      [Cfg]
      public string ErrorMode { get; set; }

      [Cfg]
      public string Arguments { get; set; }

      [Cfg]
      public string Bcc { get; set; }

      [Cfg]
      public string Before { get; set; }

      [Cfg]
      public string Cc { get; set; }

      [Cfg]
      public string Command { get; set; }

      [Cfg]
      public string Connection { get; set; }

      [Cfg]
      public string File { get; set; }

      [Cfg]
      public string From { get; set; }

      [Cfg]
      public string Html { get; set; }

      [Cfg]
      public string Method { get; set; }

      [Cfg]
      public string Mode { get; set; }

      [Cfg]
      public string NewValue { get; set; }

      [Cfg]
      public string OldValue { get; set; }

      [Cfg]
      public string Subject { get; set; }

      [Cfg]
      public string TimeOut { get; set; }

      [Cfg]
      public string To { get; set; }

      [Cfg]
      public string Url { get; set; }

      [Cfg]
      public string Id { get; set; }

      [Cfg]
      public string Body { get; set; }

      [Cfg]
      public string PlaceHolderStyle { get; set; }

      [Cfg]
      public List<NameReference> Modes { get; set; }

      [Cfg]
      public string Level { get; set; }

      [Cfg]
      public string Message { get; set; }

      [Cfg]
      public string RowCount { get; set; }

      [Cfg]
      public string Description { get; set; }

      [Cfg]
      public List<Parameter> Parameters { get; set; }

      [Cfg]
      public string Class { get; set; }

      [Cfg]
      public string Icon { get; set; }

      [Cfg]
      public string Modal { get; set; }

      public Configuration.Action ToAction() {
         var a = new Configuration.Action();
         if(bool.TryParse(this.After, out bool after)) {
            a.After = after;
         }
         a.Arguments = this.Arguments;
         a.Bcc = this.Bcc;
         if(bool.TryParse(this.Before, out bool before)) {
            a.Before = before;
         }
         a.Body = this.Body;
         a.Cc = this.Cc;
         a.Command = this.Command;
         a.Connection = this.Connection;
         a.Description = this.Description;
         a.Class = this.Class;
         a.Icon = this.Icon;
         a.ErrorMode = this.ErrorMode;
         a.File = this.File;
         a.From = this.From;
         if(bool.TryParse(this.Html, out bool html)) {
            a.Html = html;
         }
         if(int.TryParse(this.Id, out int id)) {
            a.Id = id;
         }
         a.Level = this.Level;
         a.Message = this.Message;
         a.Method = this.Method;
         a.Mode = this.Mode;
         a.Modes = this.Modes.Select(nr => nr.ToNameReference()).ToList();
         a.Name = this.Name;
         a.NewValue = this.NewValue;
         a.OldValue = this.OldValue;
         a.Parameters = this.Parameters.Select(p => p.ToParameter()).ToList();
         a.PlaceHolderStyle = this.PlaceHolderStyle;
         a.Subject = this.Subject;
         if(int.TryParse(this.TimeOut, out int timeOut)) {
            a.TimeOut = timeOut;
         }
         a.To = this.To;
         a.Type = this.Type;
         a.Url = this.Url;
         if (bool.TryParse(this.Modal, out bool modal)) {
            a.Modal = modal;
         }
         return a;
      }
   }
}
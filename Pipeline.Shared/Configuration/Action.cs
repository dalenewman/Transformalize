#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Contracts;

namespace Pipeline.Configuration {

    /// <summary>
    /// An action is performed before or after a pipeline is run.
    /// 
    /// Current actions types are: `copy`, `web`, `tfl`, `run`, and `open`.
    /// 
    /// ---
    /// 
    /// __Copy__: Copies a file `from`, `to`.
    /// 
    /// ```xml
    /// <actions>
    ///     <add type="copy" from="c:\file1.txt" to="file2.txt" />
    /// </actions>
    /// ```
    /// 
    /// ---
    /// 
    /// __Web__: Executes a GET or POST (as determined by `method`) to a specified `url`.  
    /// 
    /// * If *get*, the web request result is stored in `content`, and is forwarded to the next action's `content`. 
    /// * If *post*, `content` is posted to the `url`.
    /// 
    /// ```xml
    /// <actions>
    ///     <add type="web" 
    ///          method="get" 
    ///          url="https://www.google.com?search=blah+blah+blah..." />
    ///     <add type="web" 
    ///          method="post" 
    ///          content="blah blah blah..." 
    ///          url="https://www.google.com/index" />
    /// </actions>
    /// ```
    /// 
    /// ---
    ///
    /// __Tfl__: Execute another pipeline as determined by either `url`, `file`, or `content`. 
    /// 
    ///```xml
    /// <actions>
    ///     <add type='tfl' url='https://config.com/cfg.xml' />
    ///     <add type='tfl' file='c:\cfg.xml' />
    ///     <add type='tfl' content='{ "processes":[ { "name":"process 1" } ]}' />
    /// </actions>
    /// ```
    /// 
    /// ---
    /// 
    /// __Run__: Runs a `command` against a `connection`.
    /// 
    /// ```xml
    /// <actions>
    ///     <add type="run" connection="c1" command="update wo set status = 10 from worker wo where status = 9;" />
    /// </actions>
    /// ```
    /// 
    /// ---
    /// 
    /// __Open__: Opens a `file`.
    /// 
    /// ```xml
    /// <actions>
    ///     <add type="open" file="c:\file1.xml" />
    /// </actions>
    /// ```
    /// 
    /// </summary>
    public class Action : CfgNode {

        /// <summary>
        /// Indicates what type of action to perform.
        /// </summary>
        [Cfg(required = true, toLower = true, domain = "copy,web,tfl,run,open", ignoreCase = true)]
        public string Type { get; set; }

        [Cfg(value = true)]
        public bool After { get; set; }
        [Cfg(value = "")]
        public string Arguments { get; set; }
        [Cfg(value = "")]
        public string Bcc { get; set; }
        [Cfg(value = false)]
        public bool Before { get; set; }
        [Cfg(value = "")]
        public string Body { get; set; }
        [Cfg(value = "")]
        public string Cc { get; set; }
        [Cfg(value = "")]
        public string Command { get; set; }
        [Cfg(value = "", toLower = true)]
        public string Connection { get; set; }
        [Cfg(value = "")]
        public string File { get; set; }
        [Cfg(value = "")]
        public string From { get; set; }
        [Cfg(value = true)]
        public bool Html { get; set; }
        [Cfg(value = "get", domain = "get,post", toLower = true, ignoreCase = true)]
        public string Method { get; set; }

        [Cfg(value = "*", toLower = true)]
        public string Mode { get; set; }

        [Cfg(value = "")]
        public string NewValue { get; set; }
        [Cfg(value = "")]
        public string OldValue { get; set; }
        [Cfg(value = "")]
        public string Subject { get; set; }

        [Cfg(value = 0)]
        public int TimeOut { get; set; }

        [Cfg(value = "")]
        public string To { get; set; }

        [Cfg(value = "")]
        public string Url { get; set; }

        public bool InTemplate { get; set; }

        [Cfg(value = "")]
        public string Content { get; set; }

        [Cfg]
        public List<NameReference> Modes { get; set; }

        public string[] GetModes() {
            return Modes.Any() ? Modes.Select(m => m.Name).ToArray() : new[] { Mode };
        }

        protected override void Validate() {
            if (Before && After) {
                Error("The {0} action is set to run before AND after.  Please choose before OR after.", Type);
            }
            if (Type == "open" && File == string.Empty) {
                Error($"The {Type} action requires a file.");
            }
            if (Type == "run" && string.IsNullOrEmpty(Connection)) {
                Error("A run action needs it's connection set.");
            }
        }

        protected override void PreValidate() {
            if (Type != null && Type == "copy" && File != string.Empty && To == string.Empty) {
                To = File;
            }
        }

        [Cfg(value = "Shorthand.xml")]
        public string Shorthand { get; set; }

        /// <summary>
        /// Set for dependency injection
        /// </summary>
        public string Key { get; set; }

        public LogLevel LogLevel { get; set; } = LogLevel.Info;

    }
}
#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System;
using CommandLine;
using CommandLine.Text;

namespace Transformalize.Command {

    public class Options {

        [Option('a', "arrangement", Required = true, HelpText = "An arrangement (aka configuration) file, or url. Note: you may add an optional query string.")]
        public string Arrangement { get; set; }

        [Option('s', "schedule", Required = false, HelpText = "To use an arrangement schedule, set to 'internal', otherwise provide a cron expression in double-quotes.")]
        public string Schedule { get; set; }

        [Option('m', "mode", DefaultValue = "default", Required = false, HelpText = "A system or user-defined mode (init, check, default, etc).")]
        public string Mode { get; set; }

        [Option('f', "format", DefaultValue = "csv", Required = false, HelpText = "Output format for console provider (csv, json).")]
        public string Format { get; set; }

        [Option("phs", DefaultValue = "@()", Required = false, HelpText = "Placeholder Style: the marker, prefix, and suffix for place holders in the arrangement. Note: you can set parameters with default values in your arrangement, and also pass them in using a query string on your -a arrangement (e.g. =a \"c:\\cfg.xml?x=1&y=2\").")]
        public string PlaceHolderStyle { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public string ArrangementWithMode() {
            if (Mode == "default")
                return Arrangement;

            if (Arrangement.IndexOf("Mode=", StringComparison.OrdinalIgnoreCase) >= 0)
                return Arrangement;

            return Arrangement + (Arrangement.IndexOf('?') > 0 ? '&' : '?') + "Mode=" + Mode;
        }
    }
}
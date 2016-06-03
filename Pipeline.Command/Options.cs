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
using CommandLine;
using CommandLine.Text;
using Pipeline.Contracts;

namespace Pipeline.Command {

    public class Options {

        [Option('a', "arrangement", Required = true, HelpText = "an arrangement (aka configuration) file, or url.")]
        public string Arrangement { get; set; }

        [Option('s', "schedule", Required = false, HelpText = "a cron expression (http://www.quartz-scheduler.org/documentation/quartz-1.x/tutorials/crontrigger)")]
        public string Schedule { get; set; }

        [Option('l', "loglevel", Required = false, DefaultValue = LogLevel.Info, HelpText = "log level (i.e. none, info, debug, warn, error).")]
        public LogLevel LogLevel { get; set; }

        [Option('t', "shorthand transformations", Required = false, DefaultValue = "Shorthand.xml", HelpText = "shorthand transformations file.")]
        public string Shorthand { get; set; }

        [Option('m', "mode", DefaultValue = "default", Required = false, HelpText = "A system or user-defined mode (i.e. init, check, default, etc.). WARNING: the mode 'init' destroys and rebuilds everything.")]
        public string Mode { get; set; }

        [Option('o',"output", DefaultValue = "csv", Required = false, HelpText = "Output type (i.e. csv or json). Note: Data is only output if output connection is internal or console.")]
        public string Output { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

    }
}
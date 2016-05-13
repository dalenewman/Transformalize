#region license
// DataProfiler
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
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

        [Option('c', "configuration", Required = true, HelpText = "The configuration file, or web address.")]
        public string Configuration { get; set; }

        [Option('s', "schedule", Required = false, DefaultValue = "", HelpText = "cron expression (http://www.quartz-scheduler.org/documentation/quartz-1.x/tutorials/crontrigger)")]
        public string CronExpression { get; set; }

        [Option('l', "loglevel", Required = false, DefaultValue = LogLevel.Info, HelpText = "The log level (i.e. none, info, debug, warn, error).")]
        public LogLevel LogLevel { get; set; }

        [Option('t', "shorthand transformations", Required = false, DefaultValue = "Shorthand.xml", HelpText = "The shorthand transformations configuration file.")]
        public string Shorthand { get; set; }

        [Option('m', "mode", DefaultValue = "default", Required = false, HelpText = "A system or user-defined mode. WARNING: the mode 'init' destroys and rebuilds everything from scratch.")]
        public string Mode { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

    }
}
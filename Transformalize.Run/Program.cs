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

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging;
using Transformalize.Logging;
using Transformalize.Main;
using Process = Transformalize.Main.Process;

namespace Transformalize.Run {
    internal class Program {

        private static Options _options = new Options();

        private static void Main(string[] args) {
            var processes = new List<Process>();

            if (args.Length == 0) {
                Console.WriteLine("Please provide the process(es) name, file, or address.");
                Console.WriteLine(@"Usage:");
                Console.WriteLine(@"   tfl fancy                      - looks in tfl.exe.config for fancy process.");
                Console.WriteLine(@"   tfl c:\fancy.xml               - looks for processes in c:\fancy.xml file.");
                Console.WriteLine(@"   tfl http://localhost/fancy.xml - makes web request for processes in http://localhost/fancy.xml.");
                return;
            }

            var resource = args[0];

            var listener = new ObservableEventListener();
            listener.EnableEvents(TflEventSource.Log, EventLevel.Informational);
            var subscription = listener.LogToConsole(new LegacyLogFormatter());

            try {
                if (OptionsMayExist(args)) {
                    _options = new Options(CombineArguments(args));
                    if (_options.Valid()) {
                        if (_options.Mode.Equals("rebuild", StringComparison.OrdinalIgnoreCase)) {
                            _options.Mode = "init";
                            processes.AddRange(ProcessFactory.Create(resource, _options));
                            _options.Mode = "first";
                            processes.AddRange(ProcessFactory.Create(resource, _options));
                        } else {
                            processes.AddRange(ProcessFactory.Create(resource, _options));
                        }
                    } else {
                        foreach (var problem in _options.Problems) {
                            Console.WriteLine(resource + " | " + problem);
                        }
                        Console.WriteLine(resource + " | Aborting process.");
                        Environment.Exit(1);
                    }
                } else {
                    processes.AddRange(ProcessFactory.Create(resource));
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                listener.DisableEvents(TflEventSource.Log);
                listener.Dispose();
                return;
            }

            foreach (var process in processes) {
                try {
                    process.ExecuteScaler();
                } catch (TransformalizeException e) {
                    Console.WriteLine(e.Message);
                    break;
                }
            }

            
            listener.DisableEvents(TflEventSource.Log);
            listener.Dispose();
        }

        private static string CombineArguments(IEnumerable<string> args) {
            var options = new List<string>(args);
            options.RemoveAt(0);
            return string.Join(string.Empty, options);
        }

        private static bool OptionsMayExist(ICollection<string> args) {
            return args.Count > 1;
        }
    }
}
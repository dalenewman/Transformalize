/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Diagnostics;
using Transformalize.Libs.NLog;
using Transformalize.Model;

namespace Transformalize.Run
{
    class Program
    {

        private static readonly Stopwatch Timer = new Stopwatch();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Log.Error("Please provide the process name (e.g. Tfl MyProcess)");
                return;
            }

            var process = args[0];

            Timer.Start();

            if (OptionsMayExist(args))
            {
                var json = CombineArguments(args);
                var options = new Options(json);
                if (options.IsValid())
                {
                    new Runner(process, options).Run();
                }
                else
                {
                    foreach (var problem in options.Problems)
                    {
                        Log.Error(process + " | " + problem);
                    }
                    Log.Warn(process + " | Aborting process.");
                }

            }
            else
            {
                new Runner(process).Run();
            }

            Timer.Stop();

            Log.Info("{0} | Process completed in {1}.", process, Timer.Elapsed);

        }

        private static string CombineArguments(IEnumerable<string> args)
        {
            var options = new List<string>(args);
            options.RemoveAt(0);
            var json = string.Join(string.Empty, options);
            return json;
        }

        private static bool OptionsMayExist(ICollection<string> args)
        {
            return args.Count > 1;
        }
    }
}

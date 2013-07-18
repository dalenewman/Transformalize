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

using System.Diagnostics;
using Transformalize.NLog;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Run {
    class Program {

        private static readonly Stopwatch Timer = new Stopwatch();

        static void Main(string[] args) {

            Guard.Against(args.Length == 0, "\r\nYou must provide the process name as the first argument.\r\nE.g. tfl <process> <mode=init|delta|entity>");

            var process = args[0];
            var mode = args.Length > 1 ? args[1].ToLower() : "default";

            var logger = LogManager.GetLogger("Transformalize.Run");

            Timer.Start();
            new Runner(process, mode).Run();
            Timer.Stop();
            logger.Info("{0} | Process completed in {1}.", process, Timer.Elapsed);

        }
    }
}

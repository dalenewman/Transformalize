using System.Diagnostics;
using Transformalize.NLog;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Run {
    class Program {

        private static readonly Stopwatch Timer = new Stopwatch();

        static void Main(string[] args) {

            Guard.Against(args.Length == 0, "\r\nYou must provide the process name as the first argument.\r\nE.g. tfl <process> <mode=init|delta|entity>");

            var process = args[0];
            var mode = args.Length > 1 ? args[1].ToLower() : "delta";
            var entity = args.Length > 2 ? args[2] : string.Empty;

            Guard.Against(mode.Equals("entity") && string.IsNullOrEmpty(entity), "\r\nYou must provide an entity name when in entity mode.\r\nE.g. tfl <process> <mode> <entity>.");

            var logger = LogManager.GetLogger("Transformalize.Run");

            Timer.Start();
            new Runner(process, mode, entity).Run();
            Timer.Stop();
            logger.Info("{0} | Process completed in {1}.", process, Timer.Elapsed);

        }
    }
}

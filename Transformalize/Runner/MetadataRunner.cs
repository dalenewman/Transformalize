using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Providers.SqlServer;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner {

    public class MetadataRunner : IProcessRunner
    {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public IDictionary<string, IEnumerable<Row>> Run(Process process) {

            GlobalDiagnosticsContext.Set("process", process.Name);
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));

            var result = new Dictionary<string, IEnumerable<Row>>();

            var timer = new Stopwatch();
            timer.Start();

            if (!process.IsReady())
                return result;

            var fileName = new FileInfo(Path.Combine(Common.GetTemporaryFolder(process.Name), "MetaData.xml")).FullName;
            var writer = new MetaDataWriter(process);
            File.WriteAllText(fileName, writer.Write(), Encoding.UTF8);
            System.Diagnostics.Process.Start(fileName);

            timer.Stop();
            _log.Info("Calculated metadata in {0}.", timer.Elapsed);

            return result;
        }

        public void Dispose() {
            LogManager.Flush();
        }
    }
}
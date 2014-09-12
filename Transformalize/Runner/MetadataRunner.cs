using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner {

    public class MetadataRunner : AbstractProcessRunner, IDisposable {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public override IEnumerable<Row> Run(ref Process process) {

            SetLog(ref process);

            var result = Enumerable.Empty<Row>();

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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Runner {
    public class MetadataRunner : IProcessRunner {
        public IDictionary<string, IEnumerable<Row>> Run(Process process) {
            var result = new Dictionary<string, IEnumerable<Row>>();

            if (!process.IsReady())
                return result;

            var fileName = new FileInfo(Path.Combine(Common.GetTemporaryFolder(process.Name), "MetaData.xml")).FullName;
            var writer = new MetaDataWriter(process, new SqlServerEntityAutoFieldReader());
            File.WriteAllText(fileName, writer.Write(), Encoding.UTF8);
            System.Diagnostics.Process.Start(fileName);

            return result;
        }

        public void Dispose() {
            LogManager.Flush();
        }
    }
}
using System.IO;
using System.Text;
using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Runner
{
    public class MetadataRunner : IProcessRunner {
        public void Run(Process process) {
            if (!process.IsReady())
                return;
            var fileName = new FileInfo(Path.Combine(Common.GetTemporaryFolder(process.Name), "MetaData.xml")).FullName;
            var writer = new MetaDataWriter(process, new SqlServerEntityAutoFieldReader());
            File.WriteAllText(fileName, writer.Write(), Encoding.UTF8);
            System.Diagnostics.Process.Start(fileName);
        }

        public void Dispose() {
            LogManager.Flush();
        }
    }
}
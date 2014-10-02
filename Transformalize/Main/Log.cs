using Transformalize.Libs.NLog;
using Transformalize.Main.Providers;

namespace Transformalize.Main {

    public class Log {
        public string Name { get; set; }
        public ProviderType Provider { get; set; }
        public AbstractConnection Connection { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public LogLevel Level { get; set; }
        public string Layout { get; set; }
        public string File { get; set; }
        public string Folder { get; set; }
    }
}

using Transformalize.Libs.NLog;
using Transformalize.Main.Providers;

namespace Transformalize.Main {

    public class Log {
        private string _layout = Common.DefaultValue;
        private string _file = Common.DefaultValue;
        private string _folder = Common.DefaultValue;
        private string _name = Common.DefaultValue;
        private string _subject = Common.DefaultValue;
        private string _from = Common.DefaultValue;
        private string _to = Common.DefaultValue;
        private LogLevel _level = LogLevel.Info;
        private ProviderType _provider = ProviderType.Console;


        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        public ProviderType Provider {
            get { return _provider; }
            set { _provider = value; }
        }

        public AbstractConnection Connection { get; set; }

        public string Subject {
            get { return _subject; }
            set { _subject = value; }
        }

        public string From {
            get { return _from; }
            set { _from = value; }
        }

        public string To {
            get { return _to; }
            set { _to = value; }
        }

        public LogLevel Level {
            get { return _level; }
            set { _level = value; }
        }

        public string Layout {
            get { return _layout; }
            set { _layout = value; }
        }

        public string File {
            get { return _file; }
            set { _file = value; }
        }

        public string Folder {
            get { return _folder; }
            set { _folder = value; }
        }
    }
}

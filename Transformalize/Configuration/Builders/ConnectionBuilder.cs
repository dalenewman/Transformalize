using System.IO;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Main.Providers;

namespace Transformalize.Configuration.Builders {

    public class ConnectionBuilder {

        private readonly ProcessBuilder _processBuilder;
        private TflConnection _connection;

        public ConnectionBuilder(ProcessBuilder processBuilder, TflConnection connection) {
            _processBuilder = processBuilder;
            _connection = connection;
        }

        public ConnectionBuilder Server(string name) {
            _connection.Server = name;
            return this;
        }

        public ConnectionBuilder Database(string name) {
            _connection.Database = name;
            return this;
        }

        public ConnectionBuilder User(string userName) {
            _connection.User = userName;
            return this;
        }

        public ConnectionBuilder Password(string password) {
            _connection.Password = password;
            return this;
        }

        public ConnectionBuilder Footer(string footer) {
            _connection.Footer = footer;
            return this;
        }

        public ConnectionBuilder Encoding(string encoding) {
            _connection.Encoding = encoding;
            return this;
        }

        public ConnectionBuilder Version(string version) {
            _connection.Version = version;
            return this;
        }

        public ConnectionBuilder Header(string header) {
            _connection.Header = header;
            return this;
        }

        public ConnectionBuilder Port(int port) {
            _connection.Port = port;
            return this;
        }

        public ConnectionBuilder ConnectionString(string connectionString) {
            _connection.ConnectionString = connectionString;
            return this;
        }

        public TflProcess Process() {
            return _processBuilder.Process();
        }

        public ConnectionBuilder Connection(string name) {
            return _processBuilder.Connection(name);
        }

        public MapBuilder Map(string name) {
            return _processBuilder.Map(name);
        }

        public ConnectionBuilder Provider(string name) {
            _connection.Provider = name;
            return this;
        }

        public ConnectionBuilder Provider(ProviderType providerType) {
            _connection.Provider = providerType.ToString().ToLower();
            return this;
        }

        public ProcessBuilder TemplatePath(string path) {
            return _processBuilder.TemplatePath(path);
        }

        public ProcessBuilder ScriptPath(string path) {
            return _processBuilder.ScriptPath(path);
        }

        public TemplateBuilder Template(string name) {
            return _processBuilder.Template(name);
        }

        public SearchTypeBuilder SearchType(string name) {
            return _processBuilder.SearchType(name);
        }

        public ConnectionBuilder File(string file) {
            _connection.File = file;
            return this;
        }

        public ConnectionBuilder DateFormat(string format) {
            _connection.DateFormat = format;
            return this;
        }

        public ConnectionBuilder Folder(string folder) {
            _connection.Folder = folder;
            return this;
        }

        public ConnectionBuilder ErrorMode(ErrorMode errorMode) {
            _connection.ErrorMode = errorMode.ToString();
            return this;
        }

        public ConnectionBuilder SearchPattern(string searchPattern) {
            _connection.SearchPattern = searchPattern;
            return this;
        }

        public ConnectionBuilder Delimiter(string delimiter) {
            _connection.Delimiter = delimiter;
            return this;
        }

        public ConnectionBuilder Start(int start) {
            _connection.Start = start;
            return this;
        }

        public ConnectionBuilder SearchOption(SearchOption searchOption) {
            _connection.SearchOption = searchOption.ToString();
            return this;
        }

        public EntityBuilder Entity(string name) {
            return _processBuilder.Entity(name);
        }

        public ProcessBuilder Script(string name, string fileName) {
            return _processBuilder.Script(name, fileName);
        }

        public ActionBuilder Action(string action) {
            return _processBuilder.Action(action);
        }

        public ConnectionBuilder Connection(TflConnection element) {
            return _processBuilder.Connection(element);
        }
    }
}
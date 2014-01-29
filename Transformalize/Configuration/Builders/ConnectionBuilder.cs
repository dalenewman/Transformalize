using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Configuration.Builders {
    public class ConnectionBuilder {
        private readonly ProcessBuilder _processBuilder;
        private readonly ConnectionConfigurationElement _connection;

        public ConnectionBuilder(ProcessBuilder processBuilder, ConnectionConfigurationElement connection) {
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

        public ConnectionBuilder Port(int port) {
            _connection.Port = port;
            return this;
        }

        public ConnectionBuilder ConnectionString(string connectionString) {
            _connection.ConnectionString = connectionString;
            return this;
        }

        public ProcessConfigurationElement Process() {
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

        public ConnectionBuilder Input(IOperation operation) {
            _connection.InputOperation = operation;
            return this;
        }

        public ConnectionBuilder File(string file) {
            _connection.File = file;
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

        public EntityBuilder Entity(string name) {
            return _processBuilder.Entity(name);
        }
    }
}
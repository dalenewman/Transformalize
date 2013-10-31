namespace Transformalize.Configuration.Builders
{
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

        public ProcessConfigurationElement Process() {
            return _processBuilder.Process();
        }

        public ConnectionBuilder Connection(string name) {
            return _processBuilder.Connection(name);
        }

        public MapBuilder Map(string name) {
            return _processBuilder.Map(name);
        }
    }
}
using System;

namespace Transformalize.Main.Providers {
    public class ConnectionIs {
        private readonly AbstractConnection _connection;
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public ConnectionIs(AbstractConnection connection) {
            _connection = connection;
        }

        public bool Delimited() {
            return !string.IsNullOrEmpty(_connection.Delimiter);
        }

        public bool Excel() {
            return (_connection.Type == ProviderType.File || _connection.Type == ProviderType.Folder) && (_connection.File.EndsWith(".xlsx", IC) || _connection.File.EndsWith(".xls", IC));
        }

        public bool File() {
            return _connection.Type == ProviderType.File;
        }

        public bool Folder() {
            return _connection.Type == ProviderType.Folder;
        }

        public bool Internal() {
            return _connection.Type.Equals(ProviderType.Internal);
        }

    }
}
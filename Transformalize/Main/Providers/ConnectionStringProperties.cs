using System;
using System.Data.Common;

namespace Transformalize.Main.Providers {
    public class ConnectionStringProperties {

        private string _userProperty = string.Empty;
        private string _passwordProperty = string.Empty;
        private string _portProperty = string.Empty;
        private string _databaseProperty = string.Empty;
        private string _serverProperty = string.Empty;
        private string _trustedProperty = string.Empty;
        private string _persistSecurityInfoProperty = string.Empty;

        public string UserProperty {
            get { return _userProperty; }
            set { _userProperty = value; }
        }

        public string PasswordProperty {
            get { return _passwordProperty; }
            set { _passwordProperty = value; }
        }

        public string PortProperty {
            get { return _portProperty; }
            set { _portProperty = value; }
        }

        public string DatabaseProperty {
            get { return _databaseProperty; }
            set { _databaseProperty = value; }
        }

        public string ServerProperty {
            get { return _serverProperty; }
            set { _serverProperty = value; }
        }

        public string TrustedProperty {
            get { return _trustedProperty; }
            set { _trustedProperty = value; }
        }

        public string PersistSecurityInfoProperty {
            get { return _persistSecurityInfoProperty; }
            set { _persistSecurityInfoProperty = value; }
        }

        public string GetConnectionString(AbstractConnection connection) {

            if (string.IsNullOrEmpty(ServerProperty))
                return string.Empty;

            var builder = new DbConnectionStringBuilder { { ServerProperty, connection.Server } };

            if (!string.IsNullOrEmpty(connection.Database)) {
                builder.Add(DatabaseProperty, connection.Database);
            }

            if (!String.IsNullOrEmpty(connection.User)) {
                builder.Add(UserProperty, connection.User);
                builder.Add(PasswordProperty, connection.Password);
            } else {
                if (!String.IsNullOrEmpty(TrustedProperty)) {
                    builder.Add(TrustedProperty, true);
                }
            }

            if (PersistSecurityInfoProperty != string.Empty && connection.PersistSecurityInfo != string.Empty) {
                builder.Add(PersistSecurityInfoProperty, connection.PersistSecurityInfo);
            }

            if (connection.Port <= 0)
                return builder.ConnectionString;

            if (PortProperty == string.Empty) {
                builder[ServerProperty] += "," + connection.Port;
            } else {
                builder.Add("Port", connection.Port);
            }
            return builder.ConnectionString;
        }

    }
}
using System.Configuration;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace Transformalize.Rhino.Etl.Core.Operations {
    /// <summary>
    /// Represent an operation that uses the database can occure during the ETL process
    /// </summary>
    public abstract class AbstractDatabaseOperation : AbstractOperation {

        private readonly ConnectionStringSettings _connectionStringSettings;
        private static Hashtable _supportedTypes;
        ///<summary>
        ///The parameter prefix to use when adding parameters
        ///</summary>
        protected string ParamPrefix = "";

        /// <summary>
        /// Gets the connection string settings.
        /// </summary>
        /// <value>The connection string settings.</value>
        public ConnectionStringSettings ConnectionStringSettings {
            get { return _connectionStringSettings; }
        }

        /// <summary>
        /// Gets the name of the connection string.
        /// </summary>
        /// <value>The name of the connection string.</value>
        public string ConnectionStringName {
            get { return _connectionStringSettings.Name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDatabaseOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        protected AbstractDatabaseOperation(string connectionStringName) {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(connectionStringName),
                                             "Connection string name must have a value");

            Guard.Against<ArgumentException>(ConfigurationManager.ConnectionStrings[connectionStringName] == null,
                                             "Cannot resolve connection strings with name: " + connectionStringName);

            _connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDatabaseOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">Name of the connection string.</param>
        protected AbstractDatabaseOperation(ConnectionStringSettings connectionStringSettings) {
            Guard.Against<ArgumentException>(connectionStringSettings == null,
                                             "connectionStringSettings must resolve to a value");

            this._connectionStringSettings = connectionStringSettings;
        }

        private static void InitializeSupportedTypes() {
            _supportedTypes = new Hashtable();
            _supportedTypes[typeof(byte[])] = typeof(byte[]);
            _supportedTypes[typeof(Guid)] = typeof(Guid);
            _supportedTypes[typeof(Object)] = typeof(Object);
            _supportedTypes[typeof(Boolean)] = typeof(Boolean);
            _supportedTypes[typeof(SByte)] = typeof(SByte);
            _supportedTypes[typeof(SByte)] = typeof(SByte);
            _supportedTypes[typeof(Byte)] = typeof(Byte);
            _supportedTypes[typeof(Int16)] = typeof(Int16);
            _supportedTypes[typeof(UInt16)] = typeof(UInt16);
            _supportedTypes[typeof(Int32)] = typeof(Int32);
            _supportedTypes[typeof(UInt32)] = typeof(UInt32);
            _supportedTypes[typeof(Int64)] = typeof(Int64);
            _supportedTypes[typeof(UInt64)] = typeof(UInt64);
            _supportedTypes[typeof(Single)] = typeof(Single);
            _supportedTypes[typeof(Double)] = typeof(Double);
            _supportedTypes[typeof(Decimal)] = typeof(Decimal);
            _supportedTypes[typeof(DateTime)] = typeof(DateTime);
            _supportedTypes[typeof(String)] = typeof(String);
        }

        private static Hashtable SupportedTypes {
            get {
                if (_supportedTypes == null) {
                    InitializeSupportedTypes();
                }
                return _supportedTypes;
            }
        }

        /// <summary>
        /// Copies the row values to command parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="row">The row.</param>
        protected void CopyRowValuesToCommandParameters(IDbCommand command, Row row) {
            foreach (var column in row.Columns) {
                object value = row[column];
                if (CanUseAsParameter(value))
                    AddParameter(command, column, value);
            }
        }

        /// <summary>
        /// Adds the parameter the specifed command
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="val">The val.</param>
        protected void AddParameter(IDbCommand command, string name, object val) {
            var parameter = command.CreateParameter();
            parameter.ParameterName = ParamPrefix + name;
            parameter.Value = val ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Determines whether this value can be use as a parameter to ADO.Net provider.
        /// This perform a simple heuristic 
        /// </summary>
        /// <param name="value">The value.</param>
        private static bool CanUseAsParameter(object value) {
            if (value == null)
                return true;
            return SupportedTypes.ContainsKey(value.GetType());
        }

        /// <summary>
        /// Determines if transaction should be used
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected SqlTransaction BeginTransaction(SqlConnection connection) {
            return UseTransaction ? connection.BeginTransaction() : null;
        }

    }
}
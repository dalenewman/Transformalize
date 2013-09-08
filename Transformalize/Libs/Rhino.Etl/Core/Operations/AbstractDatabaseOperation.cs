using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Transformalize.Providers;

namespace Transformalize.Libs.Rhino.Etl.Core.Operations {
    /// <summary>
    /// Represent an operation that uses the database can occure during the ETL process
    /// </summary>
    public abstract class AbstractDatabaseOperation : AbstractOperation {
        private readonly IConnection _connection;

        private static Hashtable _supportedTypes;
        ///<summary>
        ///The parameter prefix to use when adding parameters
        ///</summary>
        protected string ParamPrefix = "";

        public IConnection Connection {
            get { return _connection; }
        }

        protected AbstractDatabaseOperation(IConnection connection)
        {
            _connection = connection;
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
            parameter.ParameterName = name.StartsWith(ParamPrefix) ? name : ParamPrefix + name;
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
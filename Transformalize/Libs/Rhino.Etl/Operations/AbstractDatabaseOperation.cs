#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Represent an operation that uses the database can occure during the ETL process
    /// </summary>
    public abstract class AbstractDatabaseOperation : AbstractOperation
    {
        private static Dictionary<Type, Type> _supportedTypes;
        private readonly AbstractConnection _connection;

        /// <summary>
        ///     The parameter prefix to use when adding parameters
        /// </summary>
        protected string ParamPrefix = string.Empty;

        protected AbstractDatabaseOperation(AbstractConnection connection)
        {
            _connection = connection;
        }

        public AbstractConnection Connection
        {
            get { return _connection; }
        }

        private static Dictionary<Type, Type> SupportedTypes
        {
            get
            {
                if (_supportedTypes == null)
                {
                    InitializeSupportedTypes();
                }
                return _supportedTypes;
            }
        }

        private static void InitializeSupportedTypes()
        {
            //_supportedTypes = new Hashtable();
            _supportedTypes = new Dictionary<Type, Type>();
            _supportedTypes[typeof (byte[])] = typeof (byte[]);
            _supportedTypes[typeof (Guid)] = typeof (Guid);
            _supportedTypes[typeof (Object)] = typeof (Object);
            _supportedTypes[typeof (Boolean)] = typeof (Boolean);
            _supportedTypes[typeof (SByte)] = typeof (SByte);
            _supportedTypes[typeof (SByte)] = typeof (SByte);
            _supportedTypes[typeof (Byte)] = typeof (Byte);
            _supportedTypes[typeof (Int16)] = typeof (Int16);
            _supportedTypes[typeof (UInt16)] = typeof (UInt16);
            _supportedTypes[typeof (Int32)] = typeof (Int32);
            _supportedTypes[typeof (UInt32)] = typeof (UInt32);
            _supportedTypes[typeof (Int64)] = typeof (Int64);
            _supportedTypes[typeof (UInt64)] = typeof (UInt64);
            _supportedTypes[typeof (Single)] = typeof (Single);
            _supportedTypes[typeof (Double)] = typeof (Double);
            _supportedTypes[typeof (Decimal)] = typeof (Decimal);
            _supportedTypes[typeof (DateTime)] = typeof (DateTime);
            _supportedTypes[typeof (String)] = typeof (String);
        }

        /// <summary>
        ///     Copies the row values to command parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="row">The row.</param>
        protected void CopyRowValuesToCommandParameters(IDbCommand command, Row row)
        {
            foreach (var column in row.Columns)
            {
                var value = row[column];
                if (CanUseAsParameter(value))
                    AddParameter(command, column, value);
            }
        }

        /// <summary>
        ///     Adds the parameter the specifed command
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="val">The val.</param>
        protected void AddParameter(IDbCommand command, string name, object val)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name.StartsWith(ParamPrefix) ? name : ParamPrefix + name;
            parameter.Value = val ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        /// <summary>
        ///     Determines whether this value can be use as a parameter to ADO.Net provider.
        ///     This perform a simple heuristic
        /// </summary>
        /// <param name="value">The value.</param>
        private static bool CanUseAsParameter(object value)
        {
            if (value == null)
                return true;
            return SupportedTypes.ContainsKey(value.GetType());
        }

        /// <summary>
        ///     Determines if transaction should be used
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected SqlTransaction BeginTransaction(SqlConnection connection)
        {
            return UseTransaction ? connection.BeginTransaction() : null;
        }
    }
}
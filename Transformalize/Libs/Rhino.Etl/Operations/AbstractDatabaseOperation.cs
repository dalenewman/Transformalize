#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections;
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
        private static Hashtable _supportedTypes;
        private readonly AbstractConnection _connection;

        /// <summary>
        ///     The parameter prefix to use when adding parameters
        /// </summary>
        protected string ParamPrefix = "";

        protected AbstractDatabaseOperation(AbstractConnection connection)
        {
            _connection = connection;
        }

        public AbstractConnection Connection
        {
            get { return _connection; }
        }

        private static Hashtable SupportedTypes
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
            _supportedTypes = new Hashtable();
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
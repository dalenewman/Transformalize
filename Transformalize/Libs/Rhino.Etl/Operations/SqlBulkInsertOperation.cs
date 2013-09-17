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
using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl.DataReaders;
using Transformalize.Libs.Rhino.Etl.Infrastructure;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Allows to execute an operation that perform a bulk insert into a sql server database
    /// </summary>
    public abstract class SqlBulkInsertOperation : AbstractDatabaseOperation
    {
        private readonly IDictionary<string, Type> _inputSchema = new Dictionary<string, Type>();

        /// <summary>
        ///     The mapping of columns from the row to the database schema.
        ///     Important: The column name in the database is case sensitive!
        /// </summary>
        public IDictionary<string, string> Mappings = new Dictionary<string, string>();

        private int _batchSize;
        private SqlBulkCopyOptions _bulkCopyOptions = SqlBulkCopyOptions.Default;
        private int _notifyBatchSize;

        /// <summary>
        ///     The schema of the destination table
        /// </summary>
        private IDictionary<string, Type> _schema = new Dictionary<string, Type>();

        private SqlBulkCopy _sqlBulkCopy;
        private int _timeout;

        protected SqlBulkInsertOperation(AbstractConnection connection, string targetTable, int timeout = 0)
            : base(connection)
        {
            Guard.Against(string.IsNullOrEmpty(targetTable), "TargetTable was not set, but it is mandatory");
            TargetTable = targetTable;
            _timeout = timeout;
        }

        /// <summary>The timeout value of the bulk insert operation</summary>
        public virtual int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>The batch size value of the bulk insert operation</summary>
        public virtual int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        /// <summary>The batch size value of the bulk insert operation</summary>
        public virtual int NotifyBatchSize
        {
            get { return _notifyBatchSize > 0 ? _notifyBatchSize : _batchSize; }
            set { _notifyBatchSize = value; }
        }

        /// <summary>The table or view to bulk load the data into.</summary>
        public string TargetTable { get; set; }

        /// <summary>
        ///     <c>true</c> to turn the <see cref="SqlBulkCopyOptions.TableLock" /> option on, otherwise <c>false</c>.
        /// </summary>
        public virtual bool LockTable
        {
            get { return IsOptionOn(SqlBulkCopyOptions.TableLock); }
            set { ToggleOption(SqlBulkCopyOptions.TableLock, value); }
        }

        /// <summary>
        ///     <c>true</c> to turn the <see cref="SqlBulkCopyOptions.KeepIdentity" /> option on, otherwise <c>false</c>.
        /// </summary>
        public virtual bool KeepIdentity
        {
            get { return IsOptionOn(SqlBulkCopyOptions.KeepIdentity); }
            set { ToggleOption(SqlBulkCopyOptions.KeepIdentity, value); }
        }

        /// <summary>
        ///     <c>true</c> to turn the <see cref="SqlBulkCopyOptions.KeepNulls" /> option on, otherwise <c>false</c>.
        /// </summary>
        public virtual bool KeepNulls
        {
            get { return IsOptionOn(SqlBulkCopyOptions.KeepNulls); }
            set { ToggleOption(SqlBulkCopyOptions.KeepNulls, value); }
        }

        /// <summary>
        ///     <c>true</c> to turn the <see cref="SqlBulkCopyOptions.CheckConstraints" /> option on, otherwise <c>false</c>.
        /// </summary>
        public virtual bool CheckConstraints
        {
            get { return IsOptionOn(SqlBulkCopyOptions.CheckConstraints); }
            set { ToggleOption(SqlBulkCopyOptions.CheckConstraints, value); }
        }

        /// <summary>
        ///     <c>true</c> to turn the <see cref="SqlBulkCopyOptions.FireTriggers" /> option on, otherwise <c>false</c>.
        /// </summary>
        public virtual bool FireTriggers
        {
            get { return IsOptionOn(SqlBulkCopyOptions.FireTriggers); }
            set { ToggleOption(SqlBulkCopyOptions.FireTriggers, value); }
        }

        /// <summary>The table or view's schema information.</summary>
        public IDictionary<string, Type> Schema
        {
            get { return _schema; }
            set { _schema = value; }
        }

        /// <summary>
        ///     Turns a <see cref="_bulkCopyOptions" /> on or off depending on the value of <paramref name="on" />
        /// </summary>
        /// <param name="option">
        ///     The <see cref="SqlBulkCopyOptions" /> to turn on or off.
        /// </param>
        /// <param name="on">
        ///     <c>true</c> to set the <see cref="SqlBulkCopyOptions" /> <paramref name="option" /> on otherwise <c>false</c> to turn the
        ///     <paramref
        ///         name="option" />
        ///     off.
        /// </param>
        protected void ToggleOption(SqlBulkCopyOptions option, bool on)
        {
            if (on)
            {
                TurnOptionOn(option);
            }
            else
            {
                TurnOptionOff(option);
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if the <paramref name="option" /> is turned on, otherwise <c>false</c>
        /// </summary>
        /// <param name="option">
        ///     The <see cref="SqlBulkCopyOptions" /> option to test for.
        /// </param>
        /// <returns></returns>
        protected bool IsOptionOn(SqlBulkCopyOptions option)
        {
            return (_bulkCopyOptions & option) == option;
        }

        /// <summary>
        ///     Turns the <paramref name="option" /> on.
        /// </summary>
        /// <param name="option"></param>
        protected void TurnOptionOn(SqlBulkCopyOptions option)
        {
            _bulkCopyOptions |= option;
        }

        /// <summary>
        ///     Turns the <paramref name="option" /> off.
        /// </summary>
        /// <param name="option"></param>
        protected void TurnOptionOff(SqlBulkCopyOptions option)
        {
            if (IsOptionOn(option))
                _bulkCopyOptions ^= option;
        }

        /// <summary>
        ///     Prepares the mapping for use, by default, it uses the schema mapping.
        ///     This is the preferred appraoch
        /// </summary>
        public virtual void PrepareMapping()
        {
            foreach (var pair in _schema)
            {
                Mappings[pair.Key] = pair.Key;
            }
        }

        /// <summary>
        ///     Use the destination Schema and Mappings to create the
        ///     operations input schema so it can build the adapter for sending
        ///     to the WriteToServer method.
        /// </summary>
        public virtual void CreateInputSchema()
        {
            foreach (var pair in Mappings)
            {
                _inputSchema.Add(pair.Key, _schema[pair.Value]);
            }
        }

        /// <summary>
        ///     Executes this operation
        /// </summary>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            Guard.Against<ArgumentException>(rows == null, "SqlBulkInsertOperation cannot accept a null enumerator");
            PrepareSchema();
            PrepareMapping();
            CreateInputSchema();
            using (var cn = (SqlConnection) Use.Connection(Connection))
            {
                _sqlBulkCopy = CreateSqlBulkCopy(cn, null);
                var adapter = new DictionaryEnumeratorDataReader(_inputSchema, rows);
                _sqlBulkCopy.WriteToServer(adapter);
            }
            yield break;
        }

        /// <summary>
        ///     Handle sql notifications
        /// </summary>
        protected virtual void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Debug("{0} rows copied to database", e.RowsCopied);
        }

        /// <summary>
        ///     Prepares the schema of the target table
        /// </summary>
        protected abstract void PrepareSchema();

        /// <summary>
        ///     Creates the SQL bulk copy instance
        /// </summary>
        private SqlBulkCopy CreateSqlBulkCopy(SqlConnection connection, SqlTransaction transaction)
        {
            var copy = new SqlBulkCopy(connection, _bulkCopyOptions, transaction)
                           {
                               BatchSize = _batchSize,
                               EnableStreaming = true
                           };
            foreach (var pair in Mappings)
            {
                copy.ColumnMappings.Add(pair.Key, pair.Value);
            }
            copy.NotifyAfter = NotifyBatchSize;
            copy.SqlRowsCopied += OnSqlRowsCopied;
            copy.DestinationTableName = TargetTable;
            copy.BulkCopyTimeout = Timeout;
            return copy;
        }
    }
}
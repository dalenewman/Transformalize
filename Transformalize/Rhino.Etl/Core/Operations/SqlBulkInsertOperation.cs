using System.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Rhino.Etl.Core.DataReaders;
using Transformalize.Rhino.Etl.Core.Infrastructure;

namespace Transformalize.Rhino.Etl.Core.Operations {
    /// <summary>
    /// Allows to execute an operation that perform a bulk insert into a sql server database
    /// </summary>
    public abstract class SqlBulkInsertOperation : AbstractDatabaseOperation {
        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        /// <summary>
        /// The schema of the destination table
        /// </summary>
        private IDictionary<string, Type> _schema = new Dictionary<string, Type>();

        /// <summary>
        /// The mapping of columns from the row to the database schema.
        /// Important: The column name in the database is case sensitive!
        /// </summary>
        public IDictionary<string, string> Mappings = new Dictionary<string, string>();
        private readonly IDictionary<string, Type> _inputSchema = new Dictionary<string, Type>();

        private SqlBulkCopy _sqlBulkCopy;
        private int _timeout;
        private int _batchSize;
        private int _notifyBatchSize;
        private SqlBulkCopyOptions _bulkCopyOptions = SqlBulkCopyOptions.Default;

        private static ConnectionStringSettings GetConnectionStringSettings(string connectionString) {
            return new ConnectionStringSettings {
                ConnectionString = connectionString,
                ProviderName = PROVIDER,
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInsertOperation"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="targetTable">The target table.</param>
        protected SqlBulkInsertOperation(string connectionString, string targetTable)
            : this(GetConnectionStringSettings(connectionString), targetTable) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInsertOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">Connection string settings to use.</param>
        /// <param name="targetTable">The target table.</param>
        protected SqlBulkInsertOperation(ConnectionStringSettings connectionStringSettings, string targetTable)
            : this(connectionStringSettings, targetTable, 0) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInsertOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="targetTable">The target table.</param>
        /// <param name="timeout">The timeout.</param>
        protected SqlBulkInsertOperation(string connectionStringName, string targetTable, int timeout)
            : this(ConfigurationManager.ConnectionStrings[connectionStringName], targetTable, timeout) {
            Guard.Against(string.IsNullOrEmpty(targetTable), "TargetTable was not set, but it is mandatory");
            this.TargetTable = targetTable;
            this._timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInsertOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">Connection string settings to use.</param>
        /// <param name="targetTable">The target table.</param>
        /// <param name="timeout">The timeout.</param>
        protected SqlBulkInsertOperation(ConnectionStringSettings connectionStringSettings, string targetTable, int timeout)
            : base(connectionStringSettings) {
            Guard.Against(string.IsNullOrEmpty(targetTable), "TargetTable was not set, but it is mandatory");
            this.TargetTable = targetTable;
            this._timeout = timeout;
        }

        /// <summary>The timeout value of the bulk insert operation</summary>
        public virtual int Timeout {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>The batch size value of the bulk insert operation</summary>
        public virtual int BatchSize {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        ///    <summary>The batch size value of the bulk insert operation</summary>
        public virtual int NotifyBatchSize {
            get { return _notifyBatchSize > 0 ? _notifyBatchSize : _batchSize; }
            set { _notifyBatchSize = value; }
        }

        /// <summary>The table or view to bulk load the data into.</summary>
        public string TargetTable { get; set; }

        /// <summary><c>true</c> to turn the <see cref="SqlBulkCopyOptions.TableLock"/> option on, otherwise <c>false</c>.</summary>
        public virtual bool LockTable {
            get { return IsOptionOn(SqlBulkCopyOptions.TableLock); }
            set { ToggleOption(SqlBulkCopyOptions.TableLock, value); }
        }

        /// <summary><c>true</c> to turn the <see cref="SqlBulkCopyOptions.KeepIdentity"/> option on, otherwise <c>false</c>.</summary>
        public virtual bool KeepIdentity {
            get { return IsOptionOn(SqlBulkCopyOptions.KeepIdentity); }
            set { ToggleOption(SqlBulkCopyOptions.KeepIdentity, value); }
        }

        /// <summary><c>true</c> to turn the <see cref="SqlBulkCopyOptions.KeepNulls"/> option on, otherwise <c>false</c>.</summary>
        public virtual bool KeepNulls {
            get { return IsOptionOn(SqlBulkCopyOptions.KeepNulls); }
            set { ToggleOption(SqlBulkCopyOptions.KeepNulls, value); }
        }

        /// <summary><c>true</c> to turn the <see cref="SqlBulkCopyOptions.CheckConstraints"/> option on, otherwise <c>false</c>.</summary>
        public virtual bool CheckConstraints {
            get { return IsOptionOn(SqlBulkCopyOptions.CheckConstraints); }
            set { ToggleOption(SqlBulkCopyOptions.CheckConstraints, value); }
        }

        /// <summary><c>true</c> to turn the <see cref="SqlBulkCopyOptions.FireTriggers"/> option on, otherwise <c>false</c>.</summary>
        public virtual bool FireTriggers {
            get { return IsOptionOn(SqlBulkCopyOptions.FireTriggers); }
            set { ToggleOption(SqlBulkCopyOptions.FireTriggers, value); }
        }

        /// <summary>Turns a <see cref="_bulkCopyOptions"/> on or off depending on the value of <paramref name="on"/></summary>
        /// <param name="option">The <see cref="SqlBulkCopyOptions"/> to turn on or off.</param>
        /// <param name="on"><c>true</c> to set the <see cref="SqlBulkCopyOptions"/> <paramref name="option"/> on otherwise <c>false</c> to turn the <paramref name="option"/> off.</param>
        protected void ToggleOption(SqlBulkCopyOptions option, bool on) {
            if (on) {
                TurnOptionOn(option);
            }
            else {
                TurnOptionOff(option);
            }
        }

        /// <summary>Returns <c>true</c> if the <paramref name="option"/> is turned on, otherwise <c>false</c></summary>
        /// <param name="option">The <see cref="SqlBulkCopyOptions"/> option to test for.</param>
        /// <returns></returns>
        protected bool IsOptionOn(SqlBulkCopyOptions option) {
            return (_bulkCopyOptions & option) == option;
        }

        /// <summary>Turns the <paramref name="option"/> on.</summary>
        /// <param name="option"></param>
        protected void TurnOptionOn(SqlBulkCopyOptions option) {
            _bulkCopyOptions |= option;
        }

        /// <summary>Turns the <paramref name="option"/> off.</summary>
        /// <param name="option"></param>
        protected void TurnOptionOff(SqlBulkCopyOptions option) {
            if (IsOptionOn(option))
                _bulkCopyOptions ^= option;
        }

        /// <summary>The table or view's schema information.</summary>
        public IDictionary<string, Type> Schema {
            get { return _schema; }
            set { _schema = value; }
        }

        /// <summary>
        /// Prepares the mapping for use, by default, it uses the schema mapping.
        /// This is the preferred appraoch
        /// </summary>
        public virtual void PrepareMapping() {
            foreach (var pair in _schema) {
                Mappings[pair.Key] = pair.Key;
            }
        }

        /// <summary>Use the destination Schema and Mappings to create the
        /// operations input schema so it can build the adapter for sending
        /// to the WriteToServer method.</summary>
        public virtual void CreateInputSchema() {
            foreach (var pair in Mappings) {
                _inputSchema.Add(pair.Key, _schema[pair.Value]);
            }
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            Guard.Against<ArgumentException>(rows == null, "SqlBulkInsertOperation cannot accept a null enumerator");
            PrepareSchema();
            PrepareMapping();
            CreateInputSchema();
            using (var connection = (SqlConnection)Use.Connection(ConnectionStringSettings))
            using (var transaction = BeginTransaction(connection)) {
                _sqlBulkCopy = CreateSqlBulkCopy(connection, transaction);
                var adapter = new DictionaryEnumeratorDataReader(_inputSchema, rows);
                _sqlBulkCopy.WriteToServer(adapter);

                if (PipelineExecuter.HasErrors) {
                    if (transaction != null) {
                        Warn("Rolling back transaction in {0}", Name);
                        transaction.Rollback();
                        Warn("Rolled back transaction in {0}", Name);
                    }
                }
                else {
                    if (transaction != null) {
                        Debug("Committing {0}", Name);
                        transaction.Commit();
                        Debug("Committed {0}", Name);
                    }
                }
            }
            yield break;
        }

        /// <summary>
        ///    Handle sql notifications
        ///    </summary>
        protected virtual void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e) {
            Debug("{0} rows copied to database", e.RowsCopied);
        }

        ///    <summary>
        /// Prepares the schema of the target table
        /// </summary>
        protected abstract void PrepareSchema();

        /// <summary>
        /// Creates the SQL bulk copy instance
        /// </summary>
        private SqlBulkCopy CreateSqlBulkCopy(SqlConnection connection, SqlTransaction transaction) {
            var copy = new SqlBulkCopy(connection, _bulkCopyOptions, transaction) { BatchSize = _batchSize };
            foreach (var pair in Mappings) {
                copy.ColumnMappings.Add(pair.Key, pair.Value);
            }
            if (IsDebugEnabled() || IsTraceEnabled()) {
                copy.NotifyAfter = NotifyBatchSize;
                copy.SqlRowsCopied += OnSqlRowsCopied;
            }
            copy.DestinationTableName = TargetTable;
            copy.BulkCopyTimeout = Timeout;
            return copy;
        }
    }
}
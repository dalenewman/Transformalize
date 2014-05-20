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
using System.Data;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.SqlServer {

    public class SqlServerConnection : AbstractConnection {
        private readonly Process _process;

        private readonly Logger _log = LogManager.GetLogger("tfl");
        public override string UserProperty { get { return "User Id"; } }
        public override string PasswordProperty { get { return "Password"; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return "Database"; } }
        public override string ServerProperty { get { return "Server"; } }
        public override string TrustedProperty { get { return "Trusted_Connection"; } }
        public override string PersistSecurityInfoProperty { get { return "Persist Security Info"; } }

        public SqlServerConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            _process = process;

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.SqlServer;
            L = "[";
            R = "]";
            IsDatabase = true;
            InsertMultipleRows = true;
            Top = true;
            NoLock = true;
            TableVariable = true;
            NoCount = true;
            IndexInclude = true;
            Views = true;
            Schemas = true;
            MaxDop = true;
        }

        public override int NextBatchId(string processName) {
            var tflEntity = new Entity(1) { Name = "TflBatch", Alias = "TflBatch", Schema = "dbo", PrimaryKey = new Fields() { new Field(FieldType.PrimaryKey) { Name = "TflBatchId" } } };
            if (!RecordsExist(tflEntity)) {
                return 1;
            }

            using (var cn = GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = "SELECT ISNULL(MAX(TflBatchId),0)+1 FROM TflBatch WHERE ProcessName = @ProcessName;";

                var process = cmd.CreateParameter();
                process.ParameterName = "@ProcessName";
                process.Value = processName;

                cmd.Parameters.Add(process);
                return (int)cmd.ExecuteScalar();
            }

        }

        public override string KeyRangeQuery(Entity entity) {

            const string sql = @"
                SELECT {0}
                FROM [{1}].[{2}] WITH (NOLOCK)
                WHERE [{3}] BETWEEN @Begin AND @End
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Schema,
                entity.Name,
                entity.Version.Name
                );
        }

        public override string KeyTopQuery(Entity entity, int top) {
            const string sql = @"
                SELECT TOP {0} {1} FROM [{2}] WITH (NOLOCK);
            ";
            return string.Format(sql, top, string.Join(", ", entity.SelectKeys(this)), entity.Name);
        }

        public override string KeyQuery(Entity entity) {

            const string sql = @"
                SELECT {0}
                FROM [{1}].[{2}] WITH (NOLOCK)
                WHERE [{3}] <= @End
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Schema,
                entity.Name,
                entity.Version.Name
            );
        }

        public override string KeyAllQuery(Entity entity) {
            const string sql = @"
                SELECT {0} FROM [{1}].[{2}] WITH (NOLOCK);
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Schema,
                entity.Name
                );
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity) {
            //default implementation for relational database
            if (entity.Inserts + entity.Updates > 0 || _process.IsFirstRun) {
                using (var cn = GetConnection()) {
                    cn.Open();

                    var cmd = cn.CreateCommand();

                    if (!entity.CanDetectChanges(input.IsDatabase)) {
                        cmd.CommandText = @"
                            INSERT INTO TflBatch(TflBatchId, ProcessName, EntityName, TflUpdate, Inserts, Updates, Deletes)
                            VALUES(@TflBatchId, @ProcessName, @EntityName, @TflUpdate, @Inserts, @Updates, @Deletes);
                        ";
                    } else {
                        var field = entity.Version.SimpleType.Replace("rowversion", "Binary").Replace("byte[]", "Binary") + "Version";
                        cmd.CommandText = string.Format(@"
                            INSERT INTO TflBatch(TflBatchId, ProcessName, EntityName, {0}, TflUpdate, Inserts, Updates, Deletes)
                            VALUES(@TflBatchId, @ProcessName, @EntityName, @End, @TflUpdate, @Inserts, @Updates, @Deletes);
                        ", field);
                    }

                    cmd.CommandType = CommandType.Text;

                    AddParameter(cmd, "@TflBatchId", entity.TflBatchId);
                    AddParameter(cmd, "@ProcessName", entity.ProcessName);
                    AddParameter(cmd, "@EntityName", entity.Alias);
                    AddParameter(cmd, "@TflUpdate", DateTime.Now);
                    AddParameter(cmd, "@Inserts", entity.Inserts);
                    AddParameter(cmd, "@Updates", entity.Updates);
                    AddParameter(cmd, "@Deletes", entity.Deletes);

                    if (entity.CanDetectChanges(input.IsDatabase)) {
                        var end = new DefaultFactory().Convert(entity.End, entity.Version.SimpleType);
                        AddParameter(cmd, "@End", end);
                    }

                    _log.Debug(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new SqlServerEntityOutputKeysExtract(this, entity);
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            return new SqlServerEntityOutputKeysExtractAll(this, entity);
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            return new SqlServerBulkLoadOperation(this, entity);
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            return new SqlServerEntityBatchUpdate(this, entity);
        }

        public override void LoadBeginVersion(Entity entity) {
            var sql = string.Format(@"
                SELECT {0}
                FROM TflBatch b
                INNER JOIN (
                    SELECT @ProcessName AS ProcessName, TflBatchId = MAX(TflBatchId)
                    FROM TflBatch
                    WHERE ProcessName = @ProcessName
                    AND EntityName = @EntityName
                ) m ON (b.ProcessName = m.ProcessName AND b.TflBatchId = m.TflBatchId);
            ", entity.GetVersionField());

            using (var cn = GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = sql;
                AddParameter(cmd, "@ProcessName", entity.ProcessName);
                AddParameter(cmd, "@EntityName", entity.Alias);

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult)) {
                    entity.HasRange = reader.Read();
                    entity.Begin = entity.HasRange ? reader.GetValue(0) : null;
                }
            }
        }

        public override void LoadEndVersion(Entity entity) {
            var sql = string.Format("SELECT MAX({0}) AS {0} FROM {1};", Enclose(entity.Version.Name), Enclose(entity.Name));

            using (var cn = GetConnection()) {
                var command = cn.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = 0;
                cn.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult)) {
                    entity.HasRows = reader.Read();
                    entity.End = entity.HasRows ? reader.GetValue(0) : null;
                }
            }
        }

        public override EntitySchema GetEntitySchema(string table, string schema = "") {
            return new DatabaseEntitySchemaReader(this).Read(table, schema);
        }
    }

    public class DatabaseEntitySchemaReader {
        private readonly AbstractConnection _connection;

        public DatabaseEntitySchemaReader(AbstractConnection connection) {
            _connection = connection;
        }

        public EntitySchema Read(string name, string schema) {
            var result = new EntitySchema();

            using (var cn = _connection.GetConnection()) {

                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = string.Format("select * from {0}{1} where 1=2;", schema.Equals(string.Empty) ? string.Empty : _connection.Enclose(schema) + ".", _connection.Enclose(name));
                var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly);
                var table = reader.GetSchemaTable();

                if (table != null) {
                    var keys = table.PrimaryKey.Any() ? table.PrimaryKey.Select(c => c.ColumnName).ToArray() : Enumerable.Empty<string>().ToArray();

                    foreach (DataRow row in table.Rows) {

                        var columnName = row["ColumnName"].ToString();

                        var field = new Field(keys.Contains(columnName) ? FieldType.PrimaryKey : FieldType.Field) {
                            Name = columnName,
                            Type = Common.ToSimpleType(row["DataType"].ToString())
                        };

                        if (field.Type.Equals("string")) {
                            field.Length = row["ColumnSize"].ToString();
                        } else {
                            field.Precision = Convert.ToInt32(row["NumericPrecision"]);
                            field.Scale = Convert.ToInt32(row["NumericScale"]);
                        }

                        if (Convert.ToBoolean(row["IsRowVersion"])) {
                            field.Length = "8";
                            field.Type = "rowversion";
                        }
                        result.Fields.Add(field);
                    }
                }

            };

            return result;

        }
    }
}
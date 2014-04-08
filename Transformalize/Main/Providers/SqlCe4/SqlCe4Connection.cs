using System;
using System.Data;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.SqlCe4 {
    public class SqlCe4Connection : AbstractConnection {

        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return "Data Source"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return "Persist Security Info"; } }

        public SqlCe4Connection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.SqlCe4;
            L = "[";
            R = "]";
            IsDatabase = true;
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
            if (entity.Inserts + entity.Updates > 0) {
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
            if (entity.Delete) {
                _log.Info("Processed {0} insert{1}, {2} update{3}, and {4} delete{5} in {6}.", entity.Inserts, entity.Inserts.Plural(), entity.Updates, entity.Updates.Plural(), entity.Deletes, entity.Deletes.Plural(), entity.Alias);
            } else {
                _log.Info("Processed {0} insert{1}, and {2} update{3} in {4}.", entity.Inserts, entity.Inserts.Plural(), entity.Updates, entity.Updates.Plural(), entity.Alias);
            }

        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            throw new System.NotImplementedException();
        }
    }
}
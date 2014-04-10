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

using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.MySql {
    public class MySqlConnection : AbstractConnection {

        public override string UserProperty { get { return "Uid"; } }
        public override string PasswordProperty { get { return "Pwd"; } }
        public override string PortProperty { get { return "Port"; } }
        public override string DatabaseProperty { get { return "Database"; } }
        public override string ServerProperty { get { return "Server"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return string.Empty; } }

        public MySqlConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.MySql;
            L = "`";
            R = "`";
            IsDatabase = true;
            Views = true;
        }

        public override string KeyAllQuery(Entity entity) {
            const string sql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0} FROM `{1}`;
                COMMIT;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Name
            );
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override string KeyQuery(Entity entity) {
            const string sql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0}
                FROM `{1}`
                WHERE `{2}` <= @End;
                COMMIT;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Name,
                entity.Version.Name
                );

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
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0}
                FROM `{1}`
                WHERE `{2}` BETWEEN @Begin AND @End;
                COMMIT;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Name,
                entity.Version.Name
            );
        }

        public override string KeyTopQuery(Entity entity, int top) {
            const string sql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0} FROM `{1}` LIMIT 0, {2};
                COMMIT;
            ";
            return string.Format(sql, string.Join(", ", entity.SelectKeys(this)), entity.Name, top);
        }
    }
}
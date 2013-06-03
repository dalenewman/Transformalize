using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Readers;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Model {

    public class Entity : WithLoggingMixin {

        public string Schema { get; set; }
        public string ProcessName { get; set; }
        public string Name { get; set; }
        public Connection InputConnection { get; set; }
        public Connection OutputConnection { get; set; }
        public Field Version;
        public Dictionary<string, Field> Keys { get; set; }
        public Dictionary<string, Field> Fields { get; set; }
        public Dictionary<string, Field> All { get; set; }

        public Entity() {
            Keys = new Dictionary<string, Field>();
            Fields = new Dictionary<string, Field>();
            All = new Dictionary<string, Field>();
        }

        public string GetKeySql(bool isRange) {
            const string sqlPattern = @"SELECT {0} FROM [{1}].[{2}] WITH (NOLOCK) WHERE {3} ORDER BY {4};";

            var criteria = string.Format(isRange ? "[{0}] BETWEEN @Start AND @End" : "[{0}] <= @End", Version.Name);
            var orderByKeys = new List<string>();
            var selectKeys = new List<string>();

            foreach (var key in Keys.Keys) {
                var field = Keys[key];
                var name = field.Name;
                var alias = field.Alias;
                selectKeys.Add(alias.Equals(name) ? string.Concat("[", name, "]") : string.Format("{0} = [{1}]", alias, name));
                orderByKeys.Add(string.Concat("[", name, "]"));
            }

            return string.Format(sqlPattern, string.Join(", ", selectKeys), Schema, Name, criteria, string.Join(", ", orderByKeys));
        }

        public IEnumerable<Dictionary<string, object>> GetKeys() {

            var keys = new List<Dictionary<string, object>>();

            var versionReader = new VersionReader(this);
            var begin = versionReader.GetBeginVersion();
            var end = versionReader.GetEndVersion();

            if (!versionReader.HasRows) {
                Warn("The entity is empty!");
                return keys;
            }

            using (var cn = new SqlConnection(InputConnection.ConnectionString)) {
                cn.Open();
                var cmd = new SqlCommand(GetKeySql(versionReader.IsRange), cn) { CommandTimeout = 0};
                
                if (versionReader.IsRange)
                    cmd.Parameters.Add(new SqlParameter("@Begin", begin));
                cmd.Parameters.Add(new SqlParameter("@End", end));
                
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        var fields = new Dictionary<string, object>();
                        for (var ordinal = 0; ordinal < reader.VisibleFieldCount; ordinal++) {
                            fields[reader.GetName(ordinal)] = reader.GetValue(ordinal);
                        }
                        keys.Add(fields);
                    }
                }
            }
            return keys;
        }

        public string CreateKeysSql(IEnumerable<Dictionary<string, object>> keyValues) {
            return SqlTemplates.CreateTableVariable("@KEYS", Keys) + SqlTemplates.BatchInsertValues("@KEYS", Keys, keyValues, InputConnection.Year, InputConnection.BatchInsertSize);
        }

    }
}
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Transformalize.Configuration {

    public class LastVersion {
        public System.Int32 VersionInt32 { get; set; }
        public System.Int64 VersionInt64 { get; set; }
        public System.Byte[] VersionByteArray { get; set; }
        public System.DateTime VersionDateTime { get; set; }
    }

    public class Entity {

        public string Schema { get; set; }
        public string Name;
        public Connection Connection;
        public Dictionary<string, Field> Keys = new Dictionary<string, Field>();
        public Dictionary<string, Field> Fields = new Dictionary<string, Field>();
        public Dictionary<string, Field> All = new Dictionary<string, Field>();
        public Field Version;

        public dynamic GetLastVersion() {
            return @"
                SELECT VersionInt32, VersionInt64, VersionByteArray, VersionDateTime
                FROM [EntityTracker]
                WHERE [EntityTrackerKey] = (
	                SELECT [EntityTrackerKey] = MAX([EntityTrackerKey])
	                FROM [EntityTracker]
	                WHERE [EntityName] = @EntityName
                );
            ";
        }

        public string GetKeySql() {
            var sqlPattern = @"SELECT {0} FROM [{1}] WITH (NOLOCK) WHERE {2} ORDER BY {0};";
            var selectKeys = new List<string>();
            var orderByKeys = new List<string>();
            var criteria = new List<string>();

            foreach (var key in Keys.Keys) {
                var field = Keys[key];
                var name = field.Name;
                var alias = field.Alias;
                selectKeys.Add(alias.Equals(name) ? string.Concat("[", name, "]") : string.Format("{0} = [{1}]", alias, name));
                orderByKeys.Add(string.Concat("[", name, "]"));
            }

            return null;
        }
    }
}
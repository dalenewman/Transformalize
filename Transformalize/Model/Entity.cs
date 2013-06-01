using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
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

        public object GetEndVersion() {
            var sql = string.Format("SELECT MAX([{0}]) AS [{0}] FROM [{1}].[{2}];", Version.Name, Schema, Name);
            object result = null;
            using (var cn = new SqlConnection(InputConnection.ConnectionString)) {
                cn.Open();
                var command = new SqlCommand(sql, cn);
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    var type = Version.SimpleType();

                    if (type.StartsWith("byte"))
                        result = reader.GetSqlBinary(0);

                    if (type.StartsWith("Date"))
                        result = reader.GetDateTime(0);

                    if (type.Equals("int32"))
                        result = reader.GetInt32(0);

                    if (type.Equals("int64"))
                        result = reader.GetInt64(0);                    
                }
            }
            return result;
        }

        public string GetEntityTrackerField() {
            var type = Version.SimpleType();

            if (type.StartsWith("byte"))
                return "VersionByteArray";

            if (type.StartsWith("Date"))
                return "VersionDateTime";

            if (type.Equals("int32"))
                return "VersionInt32";

            if (type.Equals("int64"))
                return "versionInt64";

            var message = string.Format("Version field must be Int32, Int64, Byte[], or DateTime!  Yours is {0}!", Version.Type);
            var exception = new InvalidOperationException(message);
            Error(exception, message);
            throw exception;
        }

        public bool GetBeginVersion(out object versionValue) {

            bool found;
            object result = null;

            var sql = string.Format(@"
                SELECT [{0}]
                FROM [EntityTracker]
                WHERE [EntityTrackerKey] = (
	                SELECT [EntityTrackerKey] = MAX([EntityTrackerKey])
	                FROM [EntityTracker]
	                WHERE [ProcessName] = @ProcessName 
                    AND [EntityName] = @EntityName
                );
            ", GetEntityTrackerField());

            using (var cn = new SqlConnection(OutputConnection.ConnectionString)) {
                cn.Open();
                var command = new SqlCommand(sql, cn);
                command.Parameters.Add(new SqlParameter("@ProcessName", ProcessName));
                command.Parameters.Add(new SqlParameter("@EntityName", Name));
                
                using (var reader = command.ExecuteReader())
                {
                    found = reader.Read();
                    if (found)
                    {
                        var type = Version.SimpleType();

                        if (type.StartsWith("byte"))
                            result = reader.GetByte(0);

                        if (type.StartsWith("Date"))
                            result = reader.GetDateTime(0);

                        if (type.Equals("int32"))
                            result = reader.GetInt32(0);

                        if (type.Equals("int64"))
                            result = reader.GetInt64(0);
                    }
                }
            }
            versionValue = result;
            Info("{0} | {1} last processed version for {2}.", ProcessName, found ? "Found " + versionValue + " as" : "Did not find", Name);
            return found;
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
            object begin;
            var isRange = GetBeginVersion(out begin);
            var end = GetEndVersion();
            using (var cn = new SqlConnection(InputConnection.ConnectionString)) {
                cn.Open();
                var command = new SqlCommand(GetKeySql(isRange), cn);
                if (isRange)
                    command.Parameters.Add(new SqlParameter("@Begin", begin));
                command.Parameters.Add(new SqlParameter("@End", end));

                using (var reader = command.ExecuteReader()) {
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
    }
}
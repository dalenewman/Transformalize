using System;
using System.Data;
using System.Linq;

namespace Transformalize.Main.Providers.SqlServer
{
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
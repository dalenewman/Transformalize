#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Cfg.Net.Ext;
using Dapper;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Ado {

    public class AdoSchemaReader : ISchemaReader {
        readonly IConnectionContext _c;
        readonly IConnectionFactory _cf;

        public AdoSchemaReader(IConnectionContext connection, IConnectionFactory factory) {
            _cf = factory;
            _c = connection;
        }

        private IEnumerable<Field> GetFields(string name, string query, string schema) {

            var fields = new List<Field>();

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                DataTable table = null;

                var cmd = cn.CreateCommand();
                try {
                    cmd.CommandText = query == string.Empty ? $"SELECT * FROM {(string.IsNullOrEmpty(schema) ? string.Empty : _cf.Enclose(schema) + ".")}{_cf.Enclose(name)} WHERE 1=2;" : query;
                    var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly);
                    table = reader.GetSchemaTable();
                } catch (DbException ex) {
                    _c.Error(ex.Message);
                }

                if (table == null)
                    return fields;

                foreach (DataRow row in table.Rows) {

                    var column = row["ColumnName"].ToString();
                    var ordinal = Convert.ToInt16(row["ColumnOrdinal"]);
                    var isHidden = table.Columns.Cast<DataColumn>().Any(c=>c.ColumnName == "IsHidden") && row["IsHidden"] != DBNull.Value && Convert.ToBoolean(row["IsHidden"]);
                    var dataType = Utility.ToPreferredTypeName(row["DataType"] == DBNull.Value ? "string" : ((Type)row["DataType"]).Name.ToLower());
                    var isKey = row["IsKey"] != DBNull.Value && Convert.ToBoolean(row["IsKey"]);

                    var field = fields.FirstOrDefault(f => f.Name.Equals(column, StringComparison.OrdinalIgnoreCase));

                    if (!isHidden) {
                        if (field == null) {
                            field = new Field {
                                Name = column,
                                Alias = column,
                                Ordinal = ordinal,
                                Type = dataType,
                                PrimaryKey = isKey
                            };
                            AddLengthAndPrecision(field, row);
                            fields.Add(field);
                        } else {
                            field.Type = dataType;
                            AddLengthAndPrecision(field, row);
                        }

                    }
                }
            }
            return fields;
        }

        private void AddLengthAndPrecision(Field field, DataRow row) {
            if (field.Type.In("string", "byte[]")) {

                if (row["ColumnSize"] != DBNull.Value) {
                    var size = Convert.ToInt32(row["ColumnSize"]);
                    field.Length = size == 2147483647 || size < 0 ? "max" : row["ColumnSize"].ToString();
                }

            } else if (field.Type == "decimal") {
                if (_cf.AdoProvider == AdoProvider.SqlServer && row["DataTypeName"] != null && row["DataTypeName"].ToString() == "money") {
                    field.Precision = 19;
                    field.Scale = 4;
                } else {
                    if (row["NumericPrecision"] != DBNull.Value)
                        field.Precision = Convert.ToInt32(row["NumericPrecision"]);

                    if (row["NumericScale"] != DBNull.Value)
                        field.Scale = Convert.ToInt32(row["NumericScale"]);
                }
            }
        }

        private IEnumerable<Entity> GetEntities() {
            var entities = new List<Entity>();

            string sql;
            switch (_c.Connection.Provider) {
                case "mysql":
                    sql = $"SELECT '' as table_schema, table_name from information_schema.tables where table_schema = '{_c.Connection.Database}' order by table_name";
                    break;
                case "sqlite":
                    sql = "SELECT '' as table_schema, name as table_name FROM sqlite_master WHERE type in ('table','view') ORDER by name";
                    break;
                case "postgresql":
                    sql = "select table_schema, table_name from information_schema.tables where table_schema NOT IN ('information_schema','pg_catalog') order by table_name";
                    break;
                default:
                    sql = "select table_schema, table_name from information_schema.tables order by table_name";
                    break;
            }

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                using (var reader = cn.ExecuteReader(sql)) {
                    while (reader.Read()) {
                        entities.Add(new Entity {
                            Schema = reader.GetString(0),
                            Name = reader.GetString(1),
                            Connection = _c.Connection.Name
                        });
                    }
                }
            }
            return entities;
        }

        public Schema Read() {
            var schema = new Schema { Connection = _c.Connection };
            if (_c.Connection.Table == Constants.DefaultSetting) {
                schema.Entities.AddRange(GetEntities());
                foreach (var entity in schema.Entities) {
                    entity.Fields.AddRange(GetFields(entity.Name, entity.Query, entity.Schema));
                }
            } else {
                var owner = _c.Connection.Schema == Constants.DefaultSetting ? string.Empty : _c.Connection.Schema;
                schema.Entities.Add(new Entity {
                    Name = _c.Connection.Table,
                    Schema = owner,
                    Connection = _c.Connection.Name,
                    Fields = GetFields(_c.Connection.Table, string.Empty, owner).ToList()
                });
            }
            return schema;
        }

        public Schema Read(Entity entity) {
            var schema = new Schema { Connection = _c.Connection };
            var newEntity = entity.Clone();
            newEntity.Fields = GetFields(entity.Name, entity.Query, entity.Schema).ToList();
            schema.Entities.Add(newEntity);
            return schema;
        }
    }
}

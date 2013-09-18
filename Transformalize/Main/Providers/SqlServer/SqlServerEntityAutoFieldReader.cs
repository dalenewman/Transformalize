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

using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.SqlServer
{
    public class SqlServerEntityAutoFieldReader : IEntityAutoFieldReader
    {
        private readonly IDataTypeService _dataTypeService = new SqlServerDataTypeService();
        private readonly List<Field> _fields = new List<Field>();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public Fields Read(Entity entity, bool isMaster)
        {
            using (var cn = new SqlConnection(entity.InputConnection.ConnectionString))
            {
                cn.Open();
                var cmd = new SqlCommand(PrepareSql(), cn);
                cmd.Parameters.Add(new SqlParameter("@Name", entity.Name));
                cmd.Parameters.Add(new SqlParameter("@Schema", entity.Schema));
                var reader = cmd.ExecuteReader();

                if (!reader.HasRows) return new Fields();

                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    var type = GetSystemType(reader.GetString(2));
                    var length = reader.GetString(3);
                    var fieldType = reader.GetBoolean(7) ? (isMaster ? FieldType.MasterKey : FieldType.PrimaryKey) : FieldType.Field;
                    var field = new Field(type, length, fieldType, true, string.Empty) {
                                        Name = name,
                                        Entity = entity.Name,
                                        Process = entity.ProcessName,
                                        Index = reader.GetInt32(6),
                                        Schema = entity.Schema,
                                        Input = true,
                                        Precision = reader.GetByte(4),
                                        Scale = reader.GetInt32(5),
                                        Transforms = new Transforms(),
                                        Auto = true,
                                        Alias = entity.Prefix + name
                                    };
                    _fields.Add(field);
                }
            }

            return new Fields(_fields);
        }

        private string GetSystemType(string dataType)
        {
            var typeDefined = _dataTypeService.TypesReverse.ContainsKey(dataType);
            if (!typeDefined)
            {
                _log.Warn("Transformalize hasn't mapped the SQL data type: {0} to a .NET data type.  It will default to string.", dataType);
            }
            return typeDefined ? _dataTypeService.TypesReverse[dataType] : "System.String";
        }

        private static string PrepareSql()
        {
            return @"
                SELECT
                    c.COLUMN_NAME,  --0
                    CAST(CASE c.IS_NULLABLE WHEN 'YES' THEN 1 ELSE 0 END AS BIT) AS IS_NULLABLE, --1
                    UPPER(c.DATA_TYPE) AS DATA_TYPE, --2
                    CASE UPPER(c.DATA_TYPE)
		                WHEN 'ROWVERSION' THEN '8'
		                WHEN 'TIMESTAMP' THEN '8'
		                WHEN 'NTEXT' THEN 'MAX'
		                WHEN 'IMAGE' THEN 'MAX'
		                ELSE CAST(ISNULL(c.CHARACTER_MAXIMUM_LENGTH, 0) AS NVARCHAR(4))
	                END AS CHARACTER_MAXIMUM_LENGTH, --3
                    ISNULL(c.NUMERIC_PRECISION, 0) AS NUMERIC_PRECISION, --4
                    ISNULL(c.NUMERIC_SCALE, 0) AS NUMERIC_SCALE, --5
                    ISNULL(pk.ORDINAL_POSITION,c.ORDINAL_POSITION) AS ORDINAL_POSITION, --6
                    CAST(CASE WHEN pk.COLUMN_NAME IS NULL THEN 0 ELSE 1 END AS BIT) AS IS_PRIMARY_KEY
                    ,c.*
                FROM INFORMATION_SCHEMA.COLUMNS c
                LEFT OUTER JOIN (
                    SELECT
                        kcu.TABLE_SCHEMA,
                        kcu.TABLE_NAME,
                        kcu.COLUMN_NAME,
                        kcu.ORDINAL_POSITION
                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                    INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON (kcu.TABLE_SCHEMA = tc.TABLE_SCHEMA AND kcu.TABLE_NAME = tc.TABLE_NAME AND kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY')
                ) pk ON (c.TABLE_SCHEMA = pk.TABLE_SCHEMA AND c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME)
                WHERE c.TABLE_SCHEMA = @Schema
                AND c.TABLE_NAME = @Name
                ORDER BY IS_PRIMARY_KEY DESC, c.ORDINAL_POSITION
            ";
        }
    }
}
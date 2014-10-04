using System;
using System.Data.SqlClient;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateDefaults
    {
        private Generate root;

        public GenerateDefaults(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return @"
                SELECT
	                obj.object_id,
	                obj.Name,
	                c.Name AS ColumnName,
	                OBJECT_NAME(dc.parent_object_id) AS TableName,
	                SCHEMA_NAME(obj.schema_id) AS Owner,
	                ISNULL(smobj.definition, ssmobj.definition) AS [Definition]
                FROM sys.objects obj WITH (NOLOCK)
                INNER JOIN sys.default_constraints dc WITH (NOLOCK) on (obj.object_id = dc.object_id)
                INNER JOIN sys.columns c WITH (NOLOCK) on dc.parent_object_id=c.object_id and dc.parent_column_id=c.column_id
                LEFT OUTER JOIN sys.sql_modules smobj WITH (NOLOCK) ON smobj.object_id = obj.object_id
                LEFT OUTER JOIN sys.system_sql_modules ssmobj WITH (NOLOCK) ON ssmobj.object_id = obj.object_id
                where obj.type='D';
            ";
        }

        private string AlternateName(string schema, string table, string column)
        {
            var name = string.Format("DF_{0}_{1}_{2}", schema.Replace(" ", string.Empty), table.Replace(" ", string.Empty), column.Replace(" ", string.Empty));
            var length = Math.Min(name.Length, 128);
            return name.Substring(0, length);
        }


        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterRules)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var name = reader["Name"].ToString();
                                var owner = reader["Owner"].ToString();

                                Default item = new Default(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = Util.SqlStrings.HasHexEnding(name) ? AlternateName(owner, reader["TableName"].ToString(), reader["ColumnName"].ToString()) : name;
                                item.Owner = owner;
                                item.Value = reader["Definition"].ToString();
                                database.Defaults.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}

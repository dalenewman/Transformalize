using System.Text;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates.SQLCommands
{
    internal static class ViewSQLCommand
    {

        public static string GetView(DatabaseInfo.VersionTypeEnum version)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2000 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2005 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2008 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2008R2) return GetViewSql2008();
            //Fall back to highest compatible version
            return GetViewSqlAzure();            
        }

        private static string GetViewSql2008()
        {
            return @"/* SQLoogle */ 
                SELECT DISTINCT
                    ISNULL('[' + S3.name + '].[' + Object_name(D2.object_id) + ']', '') AS DependOut, 
                    '[' + S2.name + '].[' + Object_name(D.referenced_major_id) + ']'    AS TableName, 
                    D.referenced_major_id, 
                    Objectproperty (P.object_id, 'IsSchemaBound')                       AS IsSchemaBound, 
                    P.object_id, 
                    S.name                                                              AS owner, 
                    P.name                                                              AS name, 
                    P.create_date,
                    P.modify_date
                FROM   sys.views P 
                INNER JOIN sys.schemas S ON S.schema_id = P.schema_id 
                LEFT JOIN sys.sql_dependencies D ON P.object_id = D.object_id 
                LEFT JOIN sys.objects O ON O.object_id = D.referenced_major_id 
                LEFT JOIN sys.schemas S2 ON S2.schema_id = O.schema_id 
                LEFT JOIN sys.sql_dependencies D2 ON P.object_id = D2.referenced_major_id 
                LEFT JOIN sys.objects O2 ON O2.object_id = D2.object_id 
                LEFT JOIN sys.schemas S3 ON S3.schema_id = O2.schema_id 
                ORDER  BY P.object_id;
            ";
        }

        private static string GetViewSqlAzure()
        {
            var sql = new StringBuilder();
            //Avoid using sql_dependencies. Use sys.sql_expression_dependencies instead. http://msdn.microsoft.com/en-us/library/ms174402.aspx
            sql.Append("/* SQLoogle */ SELECT DISTINCT ISNULL('[' + S3.Name + '].[' + object_name(D2.referencing_id) + ']','') AS DependOut, ");
            sql.Append("'[' + S2.Name + '].[' + object_name(D.referenced_id) + ']' AS TableName, ");
            sql.Append("D.referenced_id AS referenced_major_id, OBJECTPROPERTY (P.object_id,'IsSchemaBound') AS IsSchemaBound, ");
            sql.Append("P.object_id, S.name as owner, P.name as name ");
            sql.Append("FROM sys.views P ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = P.schema_id ");
            sql.Append("LEFT JOIN sys.sql_expression_dependencies D ON P.object_id = D.referencing_id ");
            sql.Append("LEFT JOIN sys.objects O ON O.object_id = D.referenced_id ");
            sql.Append("LEFT JOIN sys.schemas S2 ON S2.schema_id = O.schema_id ");
            sql.Append("LEFT JOIN sys.sql_expression_dependencies D2 ON P.object_id = D2.referenced_id ");
            sql.Append("LEFT JOIN sys.objects O2 ON O2.object_id = D2.referencing_id ");
            sql.Append("LEFT JOIN sys.schemas S3 ON S3.schema_id = O2.schema_id ");
            sql.Append("ORDER BY P.object_id ");
            return sql.ToString();
        }

    }
}

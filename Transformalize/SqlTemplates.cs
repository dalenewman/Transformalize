using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize {

    public static class SqlTemplates {
        private static readonly Logger Logger = LogManager.GetLogger("SqlTemplates");

        public static string TruncateTable(string name, string schema = "dbo") {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = '{0}'
        	        AND TABLE_NAME = '{1}'
                )	TRUNCATE TABLE [{0}].[{1}];
            ", schema, name);
        }

        public static string DropTable(string name, string schema = "dbo") {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = '{0}'
        	        AND TABLE_NAME = '{1}'
                )	DROP TABLE [{0}].[{1}];
            ", schema, name);
        }

        public static string CreateTable(string name, IEnumerable<string> defs, IEnumerable<string> keys) {
            var defList = string.Join(", ", defs);
            var keyList = string.Join(", ", keys);
            return string.Format(@"
                CREATE TABLE [{0}](
                    {1},
					CONSTRAINT Pk_{2} PRIMARY KEY (
						{3}
					)
                );
            ", name, defList, name.Replace(" ", string.Empty), keyList);
        }
    }
}

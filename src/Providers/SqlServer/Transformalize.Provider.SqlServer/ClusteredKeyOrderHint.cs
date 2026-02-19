using Microsoft.Data.SqlClient;
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Providers.SqlServer {
   public class ClusteredKeyOrderHint : IOrderHint {
      public void Set(SqlBulkCopy bulkCopy, Field[] fields) {
         var key = fields.FirstOrDefault(f => f.Name == Constants.TflKey);
         if (key != null) {
            bulkCopy.ColumnOrderHints.Add(key.FieldName(), SortOrder.Ascending);
         }
      }
   }
}

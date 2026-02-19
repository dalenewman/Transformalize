using Microsoft.Data.SqlClient;
using Transformalize.Configuration;

namespace Transformalize.Providers.SqlServer {
   public interface IOrderHint {
      void Set(SqlBulkCopy bulkCopy, Field[] fields);
   }
}

using System.Data.SqlClient;
using Transformalize.Model;

namespace Transformalize.Data
{
    public class SqlServerEntityBatch : IEntityBatch {
        public int GetNext(Entity entity) {
            using (var cn = new SqlConnection(entity.OutputConnection.ConnectionString)) {
                cn.Open();
                var cmd = new SqlCommand("SELECT ISNULL(MAX(TflBatchId),0)+1 FROM [dbo].[TflBatch];", cn);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
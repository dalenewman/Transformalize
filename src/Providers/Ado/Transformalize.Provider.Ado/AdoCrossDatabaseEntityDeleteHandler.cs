using Dapper;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
   /// <summary>
   /// This is a delete handler that deletes records in the output that are no longer
   /// in the input by performing a cross-database query on the same appliance / server.
   /// In other words, if DML can be written to determine and delete records in the same
   /// command then it saves Transformalize the cost of loading the input and output's keys
   /// and determining what to delete.
   /// </summary>
   public class AdoCrossDatabaseEntityDeleteHandler : IEntityDeleteHandler {

      private readonly OutputContext _context;
      private readonly IConnectionFactory _cf;

      public AdoCrossDatabaseEntityDeleteHandler(OutputContext context, IConnectionFactory cf) {
         _context = context;
         _cf = cf;
      }

      public void Delete() {
         // delete query will be different for each provider, currently that's handled in extensions on OutputContext
         var sql = _context.SqlDeleteOutputCrossDatabase(_cf, _context.Entity.BatchId);
         using(var cn = _cf.GetConnection(Constants.ApplicationName)) {
            cn.Open();
            try {
               _context.Entity.Deletes = System.Convert.ToUInt32(cn.Execute(sql));
            } catch (DbException ex) {
               _context.Error("Unable to perform cross-database delete!");
               _context.Error(ex.Message);
            }
         }
      }

      /// <summary>
      /// Violation of Interface Segregation Principle
      /// </summary>
      /// <returns></returns>
      public IEnumerable<IRow> DetermineDeletes() {
         return Enumerable.Empty<IRow>();
      }

      /// <summary>
      /// Returns true if the input and output have same provider and 
      /// server and the primary key doesn't get transformed.
      /// </summary>
      /// <param name="context"></param>
      /// <returns></returns>
      public static bool IsApplicable(Process process, Entity entity) {

         var input = process.Connections.FirstOrDefault(c => c.Name == entity.Input);
         var output = process.GetOutputConnection();
         if(input == null || output == null){
            return false;
         }

         return input.Provider == output.Provider 
                && input.Server == output.Server 
                && entity.GetPrimaryKey().All(k=>k.Transforms.Count == 0)
                && input.Provider == "sqlserver";  // only implemented for SQL Server to start
      }

   }
}

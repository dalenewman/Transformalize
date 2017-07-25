using RethinkDb;
using System;
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.RethinkDB {
    public class RethinkDbOutputController : BaseOutputController {

        IConnectionFactory _factory;
        public RethinkDbOutputController(
            OutputContext context, 
            IAction initializer, 
            IVersionDetector inputVersionDetector, 
            IVersionDetector outputVersionDetector,
            IConnectionFactory factory
            ) : base(context, initializer, inputVersionDetector, outputVersionDetector) {
            _factory = factory;
        }

        public override void Start() {
            base.Start();
            var database = Context.Connection.Database;
            var table = Context.Entity.Alias;
            var conn = _factory.Get();
            var t = Query.Db(database).Table<Dictionary<string, object>>(table);

            if (Context.Process.Mode != "init") {

                // query and set Context.Entity.BatchId (max of TflBatchId)
                var batchName = Context.Entity.TflBatchId().Alias;

                var batchId = conn.Run(t.Max(x => x[batchName]))[batchName];

                if (batchId != null) {
                    Context.Entity.BatchId = Convert.ToInt32(batchId) + 1;
                }
                Context.Info($"Found Batch Id {Context.Entity.BatchId}");

                // query and set Context.Entity.Identity (max of Identity)
                var identityName = Context.Entity.TflKey().Alias;

                var identity = conn.Run(t.Max(x => x[identityName]))[identityName];
                if (identity != null) {
                    Context.Entity.Identity = Convert.ToInt32(identity);
                }
                Context.Info($"Found Identity {Context.Entity.Identity}");
            }

            // query record count in output and use with MinVersion to determine Context.Entity.IsFirstRun
            Context.Entity.IsFirstRun = Context.Entity.MinVersion == null && conn.Run(t.Count()) == 0;
        }
    }
}

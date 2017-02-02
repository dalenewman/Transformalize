using System.Data.Common;
using System.Linq;
using Dapper;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Ado {
    public class AdoFlattenAction : IAction {
        private readonly OutputContext _output;
        private readonly IConnectionFactory _cf;

        public AdoFlattenAction(OutputContext output, IConnectionFactory cf) {
            _output = output;
            _cf = cf;
        }
        public ActionResponse Execute() {

            if (_output.Process.Entities.Sum(e => e.Inserts + e.Updates + e.Deletes) == 0) {
                return new ActionResponse(200, "nothing flattened");
            };

            var message = "0 records flattened";
            var sql = "";
            var threshold = 0;

            var fields = _output.Process.GetStarFields().SelectMany(e => e.Select(f => _cf.Enclose(f.Alias))).ToArray();
            var flat = _cf.Enclose(_output.Process.Flat);
            var star = _cf.Enclose(_output.Process.Star);

            if (_output.Process.IsFirstRun()) {
                sql = $"INSERT INTO {flat}({string.Join(",", fields)}) SELECT {string.Join(",", fields)} FROM {star};";
            } else {
                threshold = _output.Process.Entities.Select(e => e.BatchId).ToArray().Min() - 1;
                var masterEntity = _output.Process.Entities.First();
                var master = _cf.Enclose(masterEntity.OutputTableName(_output.Process.Name));
                var tflKey = _cf.Enclose(Constants.TflKey);
                var key = _cf.Enclose(masterEntity.Fields.First(f => f.Name == Constants.TflKey).FieldName());
                var batch = _cf.Enclose(masterEntity.Fields.First(f => f.Name == Constants.TflBatchId).FieldName());
                var updates = string.Join(", ", fields.Where(f => f != tflKey).Select(f => $"f.{f} = s.{f}"));

                sql = $@"
INSERT INTO {flat}({string.Join(",", fields)})
SELECT s.{string.Join(",s.", fields)}
FROM {master} m
LEFT OUTER JOIN {flat} f ON (f.{tflKey} = m.{key})
INNER JOIN {star} s ON (s.{tflKey} = m.{key})
WHERE f.{tflKey} IS NULL
AND m.{batch} > @threshold;

UPDATE f
SET {updates}
FROM {flat} f
INNER JOIN {master} m ON (m.{key} = f.{tflKey})
INNER JOIN {star} s ON (f.{tflKey} = s.{tflKey})
WHERE m.{batch} > @threshold;
";
            }

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                try {
                    _output.Debug(() => sql);
                    var count = threshold > 0 ? cn.Execute(sql, new { threshold }, commandTimeout: 0) : cn.Execute(sql, commandTimeout:0);
                    message = $"{count} record(s) flattened";
                    _output.Info(message);
                } catch (DbException ex) {
                    return new ActionResponse(500, ex.Message);
                }
            }
            return new ActionResponse(200, message);
        }
    }
}
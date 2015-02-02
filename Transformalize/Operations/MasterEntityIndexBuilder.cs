using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.Dapper;
using Transformalize.Logging;
using Transformalize.Main;
using Process = Transformalize.Main.Process;

namespace Transformalize.Operations {

    public class MasterEntityIndexBuilder {
        private readonly Process _process;

        public MasterEntityIndexBuilder(Process process) {
            _process = process;
        }

        public void Create() {
            if (string.IsNullOrEmpty(_process.Options.Mode) || _process.Options.Mode == Common.DefaultValue)
                return;
            var indexCommands = CreateRelationIndexStatements().ToArray();

            if (indexCommands.Any()) {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                using (var cn = _process.OutputConnection.GetConnection()) {
                    cn.Open();
                    foreach (var sql in indexCommands) {
                        TflLogger.Info(_process.Name, _process.MasterEntity.Name, sql);
                        try {
                            cn.Execute(sql);
                        } catch (Exception ex) {
                            TflLogger.Warn(_process.Name, _process.MasterEntity.Name, "Failed to create index. {0} {1}", sql, ex.Message);
                        }
                    }
                }
                stopWatch.Stop();
                TflLogger.Info(_process.Name, _process.MasterEntity.Name, "Indexed {0} star-schema relationships in {1}.", _process.MasterEntity.OutputName(), stopWatch.Elapsed);

            }

        }

        private IEnumerable<string> CreateRelationIndexStatements() {
            var statements = new Dictionary<string, byte>();
            foreach (var r in _process.Relationships.Where(r => r.Index)) {
                if (!JoinAndPrimaryKeyAreSame(r)) {
                    statements[CreateRelationshipIndexStatement(r)] = 1;
                }
            }
            return statements.Select(kv => kv.Key);
        }

        private bool JoinAndPrimaryKeyAreSame(Relationship relationship) {
            return relationship.Join.Select(j => j.LeftField.Alias)
                .SequenceEqual(_process.MasterEntity.PrimaryKey.Aliases());
        }

        public string CreateRelationshipIndexStatement(Relationship relationship) {
            var pattern = _process.OutputConnection.IndexInclude ?
                "CREATE INDEX {0} ON {1}.{2}({3}) INCLUDE(TflBatchId,TflKey);" :
                "CREATE INDEX {0} ON {1}.{2}({3},TflBatchId,TflKey);";

            var joinFields = relationship.Join.Select(j => j.LeftField.Alias).ToArray();
            var indexName = string.Format(
                "IX_{0}_{1}",
                _process.MasterEntity.OutputName(),
                string.Join("_", joinFields)
            ).Left(128).Trim('_');
            return string.Format(
                pattern,
                _process.OutputConnection.Enclose(indexName),
                _process.OutputConnection.Enclose(string.IsNullOrEmpty(_process.MasterEntity.Schema) ? _process.OutputConnection.DefaultSchema : _process.MasterEntity.Schema),
                _process.OutputConnection.Enclose(_process.MasterEntity.OutputName()),
                string.Join(",", joinFields.Select(_process.OutputConnection.Enclose))
            );
        }

    }
}
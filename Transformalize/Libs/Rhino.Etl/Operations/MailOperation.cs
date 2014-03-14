using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    public class MailOperation : AbstractOperation {
        private readonly Logger _log = LogManager.GetLogger("buffered-mail-output");
        private readonly List<string> _columns = new List<string>();
        private readonly string _name;

        public MailOperation(Entity entity) {
            _name = Common.EntityOutputName(entity, entity.ProcessName);
            _columns.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Output().ToArray().Select(f => f.Alias));
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            GlobalDiagnosticsContext.Set("subject", _name);

            foreach (var row in rows) {
                _log.Info(JSON.Instance.ToJSON(_columns.ToDictionary(alias => alias, alias => row[alias])));
            }
            LogManager.Flush();
            yield break;
        }
    }
}
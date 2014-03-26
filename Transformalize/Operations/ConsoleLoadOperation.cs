using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class ConsoleLoadOperation : AbstractOperation {
        private readonly List<string> _columns = new List<string>();

        public ConsoleLoadOperation(Entity entity) {
            _columns.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Output().ToArray().Select(f => f.Alias));
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                Console.WriteLine(JSON.Instance.ToJSON(_columns.ToDictionary(alias => alias, alias => row[alias])));
            }
            yield break;
        }
    }
}
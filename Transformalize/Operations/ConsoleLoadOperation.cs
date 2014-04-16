using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class ConsoleLoadOperation : AbstractOperation {
        private readonly List<string> _columns = new List<string>();
        private readonly List<string> _guids = new List<string>();
        private readonly List<string> _byteArrays = new List<string>();

        public ConsoleLoadOperation(Entity entity) {
            _columns.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Output().ToArray().Select(f => f.Alias));
            _guids.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Output().ToArray().Where(f => f.SimpleType.Equals("guid")).Select(f => f.Alias));
            _byteArrays.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Output().ToArray().Where(f => f.SimpleType.Equals("byte[]") || f.SimpleType.Equals("rowversion")).Select(f => f.Alias));
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var guid in _guids) {
                    row[guid] = ((Guid)row[guid]).ToString();
                }
                foreach (var byteArray in _byteArrays) {
                    row[byteArray] = Common.BytesToHexString((byte[])row[byteArray]);
                }
                Console.WriteLine(JSON.Instance.ToJSON(_columns.ToDictionary(alias => alias, alias => row[alias])));
            }
            yield break;
        }
    }
}
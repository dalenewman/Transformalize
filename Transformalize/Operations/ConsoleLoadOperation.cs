using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {

    public class ConsoleLoadOperation : AbstractOperation {

        private readonly List<string> _columns = new List<string>();
        private readonly List<string> _guids = new List<string>();
        private readonly List<string> _byteArrays = new List<string>();

        public ConsoleLoadOperation(Entity entity) {
            var fields = new Fields(entity.Fields, entity.CalculatedFields).WithOutput();
            _columns.AddRange(fields.Aliases());
            _guids.AddRange(fields.WithGuid().Aliases());
            _byteArrays.AddRange(fields.WithBytes().Aliases());
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var guid in _guids) {
                    row[guid] = ((Guid)row[guid]).ToString();
                }
                foreach (var byteArray in _byteArrays) {
                    row[byteArray] = Common.BytesToHexString((byte[])row[byteArray]);
                }

                Console.WriteLine(
                    JsonConvert.SerializeObject(_columns.ToDictionary(alias => alias, alias => row[alias]))
                );
            }
            yield break;
        }
    }
}
#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Load {
    public class LogLoadOperation : AbstractOperation {
        private readonly Entity _entity;

        private readonly List<string> _columns = new List<string>();
        private readonly string _name;
        private readonly List<string> _guids = new List<string>();
        private readonly List<string> _byteArrays = new List<string>();

        public LogLoadOperation(Entity entity) {
            _entity = entity;
            _name = Common.EntityOutputName(entity, entity.ProcessName);
            var fields = new Fields(entity.Fields, entity.CalculatedFields).WithOutput();
            _columns.AddRange(fields.Aliases());
            _guids.AddRange(fields.WithGuid().Aliases());
            _byteArrays.AddRange(fields.WithBytes().Aliases());
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            GlobalDiagnosticsContext.Set("output", _name);

            foreach (var row in rows) {
                foreach (var guid in _guids) {
                    row[guid] = ((Guid)row[guid]).ToString();
                }
                foreach (var byteArray in _byteArrays) {
                    row[byteArray] = Common.BytesToHexString((byte[])row[byteArray]);
                }
                TflLogger.Info(_entity.ProcessName, _entity.Name, JSON.Instance.ToJSON(_columns.ToDictionary(alias => alias, alias => row[alias])));
            }
            LogManager.Flush();
            yield break;
        }
    }
}
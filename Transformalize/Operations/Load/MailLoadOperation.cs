using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Load {
    public class MailLoadOperation : AbstractOperation {

        private readonly List<string> _columns = new List<string>();
        private readonly string _name;

        public MailLoadOperation(Entity entity) {
            _name = Common.EntityOutputName(entity, entity.ProcessName);
            _columns.AddRange(new Fields(entity.Fields, entity.CalculatedFields).WithOutput().Aliases());
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            throw new NotImplementedException();
        }
    }
}
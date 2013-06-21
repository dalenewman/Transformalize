using System.Collections.Generic;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize {
    public class RegisteredKeyExtract : AbstractOperation {
        private readonly Process _process;
        private readonly string _foreignKey;

        public RegisteredKeyExtract(Process process, string foreignKey) {
            _process = process;
            _foreignKey = foreignKey;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return _process.RelatedRows(_foreignKey);
        }
    }
}
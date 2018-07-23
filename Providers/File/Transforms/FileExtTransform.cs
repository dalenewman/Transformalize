#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Providers.File.Transforms {

    public class LineTransform : StringTransform {
        private readonly Connection _connection;
        private readonly int _lineNo;
        public LineTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.Value)) {
                return;
            }

            if (!int.TryParse(Context.Operation.Value, out _lineNo)) {
                Context.Error("A line transform must be provided a line number (an integer).");
                Run = false;
                return;
            }

            _connection = Context.Process.Connections.FirstOrDefault(c => c.Name == Context.Entity.Connection);
            if (_connection == null) {
                Run = false;
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _connection.Lines.Count >= _lineNo ? _connection.Lines[_lineNo] : string.Empty;
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("line") {
                Parameters = new List<OperationParameter>(1) {
                    new OperationParameter("value")
                }
            };
        }
    }

    public class FileExtTransform : StringTransform {
        private readonly Field _input;

        public FileExtTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }
            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            var value = (string)row[_input];
            row[Context.Field] = Path.HasExtension(value) ? Path.GetExtension(value) : string.Empty;

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("fileext");
        }
    }
}
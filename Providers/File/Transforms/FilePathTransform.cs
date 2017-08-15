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
using System.IO;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Provider.File.Transforms {

    public class FilePathTransform : BaseTransform {
        private readonly Field _input;

        public FilePathTransform(IContext context) : base(context, "string") {
            if (IsNotReceiving("string")) {
                return;
            }
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            if (Context.Transform.Extension) {
                row[Context.Field] = Path.GetFullPath((string)row[_input]);
            } else {
                var value = (string)row[_input];
                if (Path.HasExtension(value)) {
                    var path = Path.GetFullPath(value);
                    var ext = Path.GetExtension(value);
                    row[Context.Field] = path.Remove(value.Length - ext.Length);
                }
            }
            Increment();
            return row;
        }
    }
}
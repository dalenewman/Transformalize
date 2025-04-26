﻿#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Providers.File.Transforms {
    public class FileNameTransform : BaseTransform {

        private readonly Field _input;
        private readonly Func<object, string> _transform;

        public FileNameTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }
            _input = SingleInput();

            if (Context.Operation.Extension) {
                _transform = o => Path.GetFileName((string)o);
            } else {
                _transform = o => Path.GetFileNameWithoutExtension((string)o);
            }

        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row[_input]);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("filename") {
                Parameters = new List<OperationParameter>(1){
                    new OperationParameter("extension","true")
                }
            };
        }
    }
}
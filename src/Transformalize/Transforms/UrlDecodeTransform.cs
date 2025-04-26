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
using System.Collections.Generic;
#if DNF4
using System.Web;
#else
using System.Net;
#endif
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class UrlDecodeTransform : StringTransform {
        private readonly Field _input;

        public UrlDecodeTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }
            if (IsNotReceiving("string")) {
                return;
            }
            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
#if DNF4
         row[Context.Field] = HttpUtility.UrlDecode(GetString(row, _input));
#else
         row[Context.Field] = WebUtility.UrlDecode(GetString(row, _input));
#endif
         return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("urldecode")
            };
        }
    }
}

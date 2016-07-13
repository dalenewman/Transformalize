#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FormatTransform : BaseTransform, ITransform {

        readonly Field[] _input;

        public FormatTransform(IContext context)
            : base(context) {
            _input = MultipleInput();
        }

        public IRow Transform(IRow row) {
            row.SetString(Context.Field, string.Format(Context.Transform.Format, _input.Select(f => row[f]).ToArray()));
            Increment();
            return row;
        }

    }
}
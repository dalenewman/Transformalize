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
using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.System {

    public class MinDateTransform : BaseTransform {
        private readonly Field[] _dates;
        private readonly DateTime _minDate;
        private readonly bool _toString;

        public MinDateTransform(IContext context, DateTime minDate)
            : base(context, "datetime") {
            _dates = context.Entity.GetAllFields().Where(f => f.Output && f.Type.StartsWith("date", StringComparison.Ordinal)).ToArray();
            _minDate = minDate;
            _toString = context.Process.Connections.First(c => c.Name == context.Entity.Connection).Provider == "mysql";
        }

        public override IRow Transform(IRow row) {
            foreach (var date in _dates) {
                if (_toString) {
                    if (Convert.ToDateTime(row[date].ToString()) < _minDate) {
                        row[date] = _minDate;
                    }
                } else {
                    if ((DateTime)row[date] < _minDate) {
                        row[date] = _minDate;
                    }
                }
            }
            // Increment();
            return row;
        }
    }
}
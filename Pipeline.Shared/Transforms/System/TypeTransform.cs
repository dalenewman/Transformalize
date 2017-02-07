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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.System {
    public class TypeTransform : BaseTransform {
        private readonly Tuple<Field, Type>[] _fieldTypes;
        private readonly object _locker = new object();
        private byte[] _cache;


        public TypeTransform(IContext context, IEnumerable<Field> fields):base(context, null) {
            _fieldTypes = fields.Where(f => f.Type != "string").ToArray().Select(f => new Tuple<Field, Type>(f, Constants.TypeSystem()[f.Type])).ToArray();
        }

        public override IRow Transform(IRow row) {
            // only check the types on the first row because the answer is the same for every row. 
            if (_cache == null) {
                lock (_locker) {
                    var cache = new List<byte>();
                    for (var i = 0; i < _fieldTypes.Length; i++) {
                        var field = _fieldTypes[i];
                        if (row[field.Item1].GetType() != field.Item2) {
                            row[field.Item1] = field.Item1.Convert(row[field.Item1]);
                            cache.Add(1);
                        } else {
                            cache.Add(0);
                        }
                    }
                    _cache = cache.ToArray();
                }
            } else {
                for (var i = 0; i < _cache.Length; i++) {
                    var field = _fieldTypes[i];
                    if (_cache[i] == 1) {
                        row[field.Item1] = field.Item1.Convert(row[field.Item1]);
                    }
                }

            }
            return row;
        }

    }
}
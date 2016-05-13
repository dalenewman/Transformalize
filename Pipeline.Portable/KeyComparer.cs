#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
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
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline {
    public class KeyComparer : IEqualityComparer<IRow> {
        private readonly Field[] _keys;
        private readonly Func<IRow, int> _hasher;

        public KeyComparer(Field[] keys) {
            _keys = keys;
            if (_keys.Length == 1) {
                _hasher = row => row[_keys[0]].GetHashCode();
            } else {
                _hasher = row => {
                    unchecked {
                        return _keys.Aggregate(17, (current, key) => current * 23 + row[key].GetHashCode());
                    }
                };
            }
        }

        public bool Equals(IRow x, IRow y) {
            return x.Match(_keys, y);
        }

        public int GetHashCode(IRow obj) {
            return _hasher(obj);
        }

    }
}
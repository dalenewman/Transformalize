#region license
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
using Transformalize.Contracts;

namespace Transformalize.Impl {
    public class RowFactory : IRowFactory {
        private readonly int _capacity;
        private readonly bool _isMaster;
        private readonly bool _keys;

        public RowFactory(int capacity, bool isMaster, bool keys) {
            _capacity = capacity;
            _isMaster = isMaster;
            _keys = keys;
        }

        public IRow Create() {
            if (_keys)
                return new KeyRow(_capacity);

            return _isMaster ? new MasterRow(_capacity) : (IRow)new SlaveRow(_capacity);
        }

        public IRow Clone(IRow row, IEnumerable<IField> fields) {
            var newRow = Create();
            foreach (var field in fields) {
                newRow[field] = row[field];
            }
            return newRow;
        }
    }
}
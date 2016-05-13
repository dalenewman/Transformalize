#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
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
using Pipeline.Contracts;

namespace Pipeline {
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
            if(_keys)
                return new KeyRow(_capacity);

            return _isMaster ? new MasterRow(_capacity) : (IRow)new SlaveRow(_capacity);
        }
    }
}
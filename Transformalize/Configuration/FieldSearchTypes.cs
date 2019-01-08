#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Configuration {
    public class FieldSearchTypes : IEnumerable<FieldSearchType> {
        private readonly List<FieldSearchType> _fieldSearchTypes = new List<FieldSearchType>();

        public FieldSearchTypes(Process process, IEnumerable<Field> fields) {
            foreach (var field in fields) {
                var searchType = process.SearchTypes.FirstOrDefault(st => st.Name == field.SearchType);

                _fieldSearchTypes.Add(new FieldSearchType {
                    Alias = field.Alias,
                    Field = field,
                    SearchType = searchType ?? new SearchType {
                        Name = "none",
                        MultiValued = false,
                        Store = false,
                        Index = false
                    }
                });
            }
        }

        public IEnumerator<FieldSearchType> GetEnumerator() {
            return _fieldSearchTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
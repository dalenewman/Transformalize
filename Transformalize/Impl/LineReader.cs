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

using System.Collections.Generic;
using System.IO;
using Transformalize.Contracts;

namespace Transformalize.Impl {

    public class LineReader : IReadLines {
        private readonly string _content;
        private readonly int _count;

        public LineReader(string content, int count = 0) {
            _content = content;
            _count = count;
        }

        public IEnumerable<string> Read() {
            var lines = new List<string>();
            using (var reader = new StringReader(_content)) {

                if (_count == 0) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        lines.Add(line);
                    }
                } else {
                    for (var i = 0; i < _count + 1; i++) {
                        string line;
                        if ((line = reader.ReadLine()) == null) {
                            break;
                        }
                        lines.Add(line);
                    }

                }

            }
            return lines;
        }
    }
}
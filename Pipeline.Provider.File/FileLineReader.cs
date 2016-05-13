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
using System.Collections.Generic;
using System.IO;
using Pipeline.Contracts;

namespace Pipeline.Provider.File {
    public class FileLineReader : IReadLines {
        private readonly FileInfo _fileInfo;
        private readonly int _count;

        public FileLineReader(FileInfo fileInfo, int count = 100) {
            _fileInfo = fileInfo;
            _count = count;
        }

        public IEnumerable<string> Read() {
            var lines = new List<string>();
            using (var reader = new StreamReader(_fileInfo.FullName)) {
                for (var i = 0; i < _count + 1; i++) {
                    if (reader.EndOfStream) {
                        break;
                    }
                    var line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line)) {
                        lines.Add(line);
                    }
                }
            }
            return lines.ToArray();
        }
    }
}

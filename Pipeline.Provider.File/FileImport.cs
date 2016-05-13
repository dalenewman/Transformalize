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
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Provider.File {
    public class FileImport : ICreateConfiguration {

        private readonly FileInfo _fileInfo;
        private readonly char _delimiter;
        private readonly bool _hasColumnNames;
        private readonly Field[] _fields;

        public FileImport(FileInfo fileInfo, char delimiter, bool hasColumnNames, Field[] fields) {
            _fileInfo = fileInfo;
            _delimiter = delimiter;
            _hasColumnNames = hasColumnNames;
            _fields = fields;
        }

        public string Create() {

            var name = Utility.Identifier(_fileInfo.Name);

            return new Process {
                Name = name,
                Star = name,
                Pipeline = "parallel.linq",
                Connections = new List<Connection> {
                        new Connection{
                            Name = "input",
                            Provider = "file",
                            File = _fileInfo.FullName,
                            Delimiter = _delimiter == default(char) ? "," : _delimiter.ToString(),
                            Start = _hasColumnNames ? 2 : 1
                        }
                    },
                Entities = new List<Entity> {
                        new Entity{
                            Name = name,
                            Connection = "input",
                            PrependProcessNameToOutputName = false,
                            Fields = _fields.ToList()
                        }
                    }
            }.Serialize();

        }

    }
}
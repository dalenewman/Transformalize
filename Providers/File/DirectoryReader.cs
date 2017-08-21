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
using System.IO;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.File {
    public class DirectoryReader : IReadInputKeysAndHashCodes {

        private readonly InputContext _input;
        private readonly IRowFactory _rowFactory;

        public DirectoryReader(InputContext input, IRowFactory rowFactory) {
            _input = input;
            _rowFactory = rowFactory;
        }

        public IEnumerable<IRow> Read() {

            var searchOption = (SearchOption)Enum.Parse(typeof(SearchOption), _input.Connection.SearchOption, true);
            _input.Info($"Searching folder: {_input.Connection.Folder}");

            var files = new DirectoryInfo(_input.Connection.Folder).GetFiles(_input.Connection.SearchPattern, searchOption).OrderBy(f=>f.LastAccessTimeUtc).ToArray();
            _input.Info($"Found {files.Length} files.");

            var names = _input.InputFields.Select(f => f.Name.ToLower()).ToArray();

            foreach (var file in files) {
                var row = _rowFactory.Create();
                for (var i = 0; i < _input.InputFields.Length; i++) {
                    var field = _input.InputFields[i];
                    switch (names[i]) {
                        case "creation":
                        case "created":
                        case "creationtime":
                        case "creationtimeutc":
                            row[field] = file.CreationTimeUtc;
                            break;
                        case "folder":
                        case "foldername":
                        case "directory":
                        case "directoryname":
                            row[field] = file.DirectoryName ?? string.Empty;
                            break;
                        case "ext":
                        case "extension":
                            row[field] = file.Extension;
                            break;
                        case "fullname":
                            row[field] = file.FullName;
                            break;
                        case "lastwrite":
                        case "updated":
                        case "lastwritetime":
                        case "lastwritetimeutc":
                            row[field] = file.LastWriteTimeUtc;
                            break;
                        case "lastaccess":
                        case "accessed":
                        case "lastaccesstime":
                        case "lastaccesstimeutc":
                            row[field] = file.LastAccessTimeUtc;
                            break;
                        case "size":
                        case "bytes":
                        case "length":
                            if (field.Type != "long" && field.Type != "int64") {
                                _input.Warn($"The {names[i]} input is type long, but field is setup as {field.Type}.");
                            }
                            row[field] = file.Length;
                            break;
                        case "name":
                            row[field] = file.Name;
                            break;
                        case "readonly":
                        case "isreadonly":
                            row[field] = file.IsReadOnly;
                            break;
                    }
                }

                yield return row;
            }

        }
    }
}
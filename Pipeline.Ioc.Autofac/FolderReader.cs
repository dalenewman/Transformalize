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
using Cfg.Net.Ext;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Providers.File;

namespace Transformalize.Ioc.Autofac {
    public class FolderReader : IRead {

        private readonly IRead _reader;
        public FolderReader(IConnectionContext input, IRowFactory rowFactory) {

            var readers = new List<IRead>();
            var searchOption = (SearchOption)Enum.Parse(typeof(SearchOption), input.Connection.SearchOption, true);

            input.Info($"Searching folder: {input.Connection.Folder}");
            var files = new DirectoryInfo(input.Connection.Folder).GetFiles(input.Connection.SearchPattern, searchOption).OrderBy(f => f.CreationTime).ToArray();

            input.Info($"Found {files.Length} files.");
            foreach (var file in files) {
                input.Info($"Found file: {file.Name}");

                var context = new PipelineContext(input.Logger, input.Process, input.Entity, input.Field, input.Operation);

                var fileConnection = input.Connection.Clone();
                fileConnection.Provider = "file";
                fileConnection.File = file.FullName;

                var fileInput = new InputContext(context) { Connection = fileConnection };

                readers.Add(new DelimitedFileReader(fileInput, rowFactory));
            }
            _reader = new CompositeReader(readers);
        }

        public IEnumerable<IRow> Read() {
            return _reader.Read();
        }
    }
}
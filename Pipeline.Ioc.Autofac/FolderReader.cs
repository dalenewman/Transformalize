#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
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
using System;
using System.Collections.Generic;
using System.IO;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Nulls;
using Pipeline.Provider.Excel;
using Pipeline.Provider.File;

namespace Pipeline.Ioc.Autofac
{
    public class FolderReader : IRead {

        private readonly IRead _reader;
        public FolderReader(InputContext input, IRowFactory rowFactory) {
            var readers = new List<IRead>();
            var searchOption = (SearchOption)Enum.Parse(typeof(SearchOption), input.Connection.SearchOption, true);
            input.Info($"Searching folder: {input.Connection.Folder}");
            var files = new DirectoryInfo(input.Connection.Folder).GetFiles(input.Connection.SearchPattern, searchOption);
            input.Info($"Found {files.Length} files.");
            foreach (var file in files) {
                input.Info($"Found file: {file.Name}");
                if (file.Extension.ToLower().Contains("xls")) {
                    readers.Add(new ExcelReader(input, rowFactory));
                } else {
                    readers.Add(new DelimitedFileReader(input, rowFactory, new NullRowCondition()));
                }
            }
            _reader = new CompositeReader(readers);
        }

        public IEnumerable<IRow> Read() {
            return _reader.Read();
        }
    }
}
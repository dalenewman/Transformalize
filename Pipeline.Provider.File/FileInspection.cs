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
using System.Linq;
using Cfg.Net.Ext;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Extensions;

namespace Pipeline.Provider.File {
    public class FileInspection : ICreateConfiguration {

        private readonly IConnectionContext _context;
        private readonly FileInfo _fileInfo;
        private readonly int _lines;

        public FileInspection(
            IConnectionContext context,
            FileInfo fileInfo,
            int lines = 100) {
            _context = context;
            _fileInfo = fileInfo;
            _lines = lines;
        }

        public string Create() {

            var identifier = Utility.Identifier(_fileInfo.Name.Replace(_fileInfo.Extension, string.Empty));
            var quoted = _fileInfo.Extension.ToLower() == ".csv";

            var lines = new FileLineReader(_fileInfo, _lines).Read().ToArray();
            var delimiter = Utility.FindDelimiter(lines, _context.Connection.Delimiters, quoted);

            var values = lines.First()
                .SplitLine(delimiter, quoted)
                .Select(c => c.Trim('"'))
                .Select(c => c.Trim())
                .ToArray();

            // substitute blank headers with excel column names (useful when some of the column headers are blank)
            for (var i = 0; i < values.Length; i++) {
                if (values[i] == string.Empty) {
                    values[i] = Utility.GetExcelName(i);
                }
            }

            var hasColumnNames = ColumnNames.AreValid(_context, values);
            var fieldNames = hasColumnNames ? values : ColumnNames.Generate(values.Length).ToArray();
            var connection = new Connection {
                Name = "input",
                Provider = "file",
                File = _fileInfo.FullName,
                Delimiter = delimiter == default(char) ? "," : delimiter.ToString(),
            }.WithDefaults();

            connection.Start = hasColumnNames ? 1 : 0;  // set after WithDefaults()

            var process = new Process {
                Name = "FileInspector",
                Pipeline = "parallel.linq",
                Connections = new List<Connection> { connection }
            }.WithDefaults();

            process.Entities.Add(new Entity {
                Name = identifier,
                PrependProcessNameToOutputName = false,
                Sample = Convert.ToInt32(_context.Connection.Sample)
            }.WithDefaults());

            foreach (var name in fieldNames) {
                process.Entities[0].Fields.Add(new Field {
                    Name = name,
                    Alias = Constants.InvalidFieldNames.Contains(name) ? identifier + name : name,
                    Length = "max"
                }.WithDefaults());
            }

            return process.Serialize();

        }

    }
}

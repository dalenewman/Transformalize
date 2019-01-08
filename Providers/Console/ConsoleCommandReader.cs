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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;

namespace Transformalize.Providers.Console {
    public class ConsoleCommandReader : IRead {
        private readonly InputContext _input;
        private readonly IRowFactory _rowFactory;
        private readonly IField _inputField;

        public ConsoleCommandReader(InputContext input, IRowFactory rowFactory) {
            _rowFactory = rowFactory;
            _input = input;
            _inputField = input.InputFields.FirstOrDefault();
        }

        public IEnumerable<IRow> Read() {

            if (_inputField == null) {
                _input.Error("You must have one input field for console provider input.");
                yield break;
            }

            // Start the child process.
            var p = new Process {
                StartInfo = {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = _input.Connection.Command,
                    Arguments = _input.Connection.Arguments
                }
            };

            // Redirect the output stream of the child process.
            p.Start();

            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            var output = p.StandardOutput.ReadToEnd();

            var lineNumber = 1;
            foreach (var line in new LineReader(output).Read()) {

                if (line == string.Empty || lineNumber < _input.Connection.Start) {
                    lineNumber++;
                    continue;
                }

                if (_input.Connection.End > 0 && lineNumber > _input.Connection.End) {
                    yield break;
                }

                var row = _rowFactory.Create();
                row[_inputField] = line;
                lineNumber++;
                yield return row;
            }


            p.WaitForExit();
        }

    }
}
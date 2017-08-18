using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Console
{
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
                    FileName = _input.Connection.Command
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
            foreach (var line in new LineReader(output).Read())
            {

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
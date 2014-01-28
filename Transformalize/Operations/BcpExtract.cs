using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Operations.Extract;

namespace Transformalize.Operations
{
    public class BcpExtract : AbstractOperation {

        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly string _folder;
        private readonly FileInfo _fileInfo;

        public BcpExtract(Process process, Entity entity) {
            _process = process;
            _entity = entity;

            _folder = Common.GetTemporaryFolder(process.Name) + @"\Temp\";
            _fileInfo = new FileInfo(_folder + entity.OutputName() + ".txt");

            PrepareFileSystem();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var cn = _entity.InputConnection;
            var columns = string.Join(",", new FieldSqlWriter(_entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).Select(f => cn.Provider.Enclose(f.Name)));
            var query = string.Format("SELECT {0} FROM [{1}].[{2}].[{3}];", columns, cn.Database, _entity.Schema, _entity.Name);
            var credentials = cn.IsTrusted() ? "-T" : string.Format("-U {0} -P \"{1}\"", cn.User, cn.Password);
            var arguments = string.Format("\"{0}\" queryout \"{1}\" -S \"{2}\" {3} -b {4} -w", query, _fileInfo.FullName, _entity.InputConnection.Server, credentials, _entity.InputConnection.BatchSize);

            _log.Debug("Running bcp " + arguments);

            var bcp = new System.Diagnostics.Process { 
                StartInfo = {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = FindBcp(),
                    Arguments = arguments,
                    CreateNoWindow = true
                }
            };

            bcp.OutputDataReceived += (sender, args) => LogLine(args.Data);
            bcp.Start();
            bcp.BeginOutputReadLine();

            bcp.WaitForExit();

            //todo: check output for errors and row count
            _entity.InputConnection.File = _fileInfo.FullName;
            _entity.InputConnection.Delimiter = "\t";

            var fileImport = new FileDelimitedExtract(_entity, 0);
            fileImport.PrepareForExecution(PipelineExecuter);
            return fileImport.Execute(null);
        }

        private void LogLine(string line) {
            _log.Debug(line);
        }

        private void PrepareFileSystem() {
            if (!Directory.Exists(_folder)) {
                _log.Debug("Creating temporary folder.");
                Directory.CreateDirectory(_folder);
            } else {
                if (_fileInfo.Exists) {
                    _log.Debug("Deleting temporary file {0}", _fileInfo.Name);
                    _fileInfo.Delete();
                }
            }
        }

        private string FindBcp() {

            if (!String.IsNullOrEmpty(_process.Bcp)) {
                var fileInfo = new FileInfo(_process.Bcp);
                if (fileInfo.Exists) {
                    return fileInfo.FullName;
                }
            }

            var roots = new[] { @"C:\Program Files\", @"C:\Program Files (x86)\" };
            var versions = new[] { "110", "100", "90" };

            foreach (var root in roots) {
                foreach (var version in versions) {
                    var fileInfo = new FileInfo(root + @"Microsoft SQL Server\" + version + @"\Tools\Binn\bcp.exe");
                    if (fileInfo.Exists) {
                        _log.Debug("Found bcp at {0}", fileInfo.FullName + ".");
                        return fileInfo.FullName;
                    }
                }
            }

            _log.Error("Can't find BCP Utility.\r\nIf you want to use BCP, set bcp attribute in <process/> element to the full path and file name of bcp.exe.\r\nAlso, make sure you've installed the feature pack or client tools for SQL Server that include bcp.exe.");
            Environment.Exit(1);
            return null;
        }

    }
}
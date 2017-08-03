using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices.AdomdClient;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.SSAS {

    public class SSASOutputProvider : IOutputProvider {
        readonly OutputContext _output;
        readonly InputContext _input;

        public SSASOutputProvider(InputContext input, OutputContext output) {
            _input = input;
            _output = output;
        }

        public void Delete() {
            throw new NotImplementedException();
        }

        public object GetMaxVersion() {

            var ids = new SSASIdentifiers(_input, _output);

            var versionField = _output.Entity.GetVersionField();
            if (versionField == null)
                return null;

            if (_output.Process.Mode == "init")
                return null;

            object result = null;

            using (AdomdConnection conn = new AdomdConnection($"Data Source={_output.Connection.Server};Catalog={ids.DatabaseId}")) {
                conn.Open();
                var mdx = $"select [MEASURES].[{ids.VersionId}] ON COLUMNS FROM [{ids.CubeId}]";
                using (var cmd = new AdomdCommand(mdx, conn)) {
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            result = reader[0];
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }

            return result;

        }

        public void End() {
            throw new NotImplementedException();
        }

        public int GetNextTflBatchId() {
            return 0;
        }

        public int GetMaxTflKey() {
            return 0;
        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public void Start() {
            throw new NotImplementedException();
        }

        public void Write(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}

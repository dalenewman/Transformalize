using Microsoft.AnalysisServices.AdomdClient;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.SSAS {
    public class SSASOutputVersionDetector : IVersionDetector {
        readonly OutputContext _output;
        readonly InputContext _input;

        public SSASOutputVersionDetector(InputContext input, OutputContext output) {
            _input = input;
            _output = output;
        }
        public object Detect() {

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
    }
}

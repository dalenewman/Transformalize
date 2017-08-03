using Transformalize.Context;

namespace Transformalize.Provider.SSAS {

    public class SSASIdentifiers {
        public SSASIdentifiers(InputContext input, OutputContext output) {
            DatabaseId = output.Connection.Database;
            DataSourceId = input.Connection.Database;
            DataSourceViewId = input.Process.Name;
            CubeId = input.Process.Name;
            VersionId = "Version";
            NormalMeasureGroupId = "Normal";
            DistinctMeasureGroupId = "Distinct";
        }
        public string DatabaseId { get; }
        public string DataSourceId { get; }
        public string DataSourceViewId { get; }
        public string CubeId { get; }
        public string VersionId { get; set; }
        public string NormalMeasureGroupId { get; set; }
        public string DistinctMeasureGroupId { get; set; }
    }
}

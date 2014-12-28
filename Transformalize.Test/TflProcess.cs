using Transformalize.Libs.NanoXml;

namespace Transformalize.Test {
    public class TflProcess : TflNode {

        public TflProcess(NanoXmlNode node)
            : base(node) {

            Key("name");

            Property("enabled", true);
            Property("inherit", string.Empty);
            Property("mode", string.Empty);
            Property("parallel", true);
            Property("pipeline-threading", string.Empty);
            Property("star", string.Empty);
            Property("star-enabled", true);
            Property("template-content-type", "raw");
            Property("time-zone", string.Empty);
            Property("view", string.Empty);
            Property("view-enabled", true);

            Class<TflAction>("actions");
            Class<TflCalculatedField>("calculated-fields");
            Class<TflConnection>("connections", true);
            Class<TflEntity>("entities", true);
            Class<TflFileInspection>("file-inspection");
            Class<TflLog>("log");
            Class<TflMap>("maps");
            Class<TflParameter>("parameters");
            Class<TflProvider>("providers");
            Class<TflRelationship>("relationships");
            Class<TflScript>("scripts");
            Class<TflSearchType>("search-types");
            Class<TflTemplate>("templates");
        }


    }
}
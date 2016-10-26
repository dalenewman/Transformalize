using System.Xml.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FormatXmlTransfrom : BaseTransform {
        private readonly Field _input;

        public FormatXmlTransfrom(IContext context) : base(context, "string") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            var xml = row[_input] as string;
            row[Context.Field] = string.IsNullOrEmpty(xml) ? string.Empty : XDocument.Parse(xml).ToString();
            Increment();
            return row;
        }
    }
}
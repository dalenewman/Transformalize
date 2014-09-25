using System.Collections.Generic;
using System.Text;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public abstract class TemplateOperation : ShouldRunOperation {

        private readonly StringBuilder _builder = new StringBuilder();
        protected readonly string OutType;
        protected readonly IParameters Parameters;
        protected readonly string Template;

        protected TemplateOperation(string outKey, string outType, string template, IEnumerable<KeyValuePair<string, Template>> templates, IParameters parameters)
            : base(string.Empty, outKey) {
            OutType = Common.ToSimpleType(outType);
            Parameters = parameters;

            CombineTemplates(templates, ref _builder);
            _builder.Append(template);
            Template = _builder.ToString();
        }

        private static void CombineTemplates(IEnumerable<KeyValuePair<string, Template>> templates, ref StringBuilder builder) {
            foreach (var pair in templates) {
                builder.AppendLine(pair.Value.Contents.Content);
            }
        }

    }
}
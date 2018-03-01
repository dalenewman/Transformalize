
using System;
using System.Dynamic;
using System.Web.Mvc;
using Orchard.DisplayManagement.Implementation;
using Orchard.Templates.Services;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Pipeline.Web.Orchard.Impl {
    public class OrchardRazorTransform : BaseTransform {
        private readonly ITemplateProcessor _processor;

        private readonly Field[] _input;

        public OrchardRazorTransform(IContext context, ITemplateProcessor processor) : base(context, context.Field.Type) {
            _processor = processor;
            _input = MultipleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = Context.Field.Convert(_processor.Process(Context.Operation.Template, Context.Key, null, row.ToFriendlyExpandoObject(_input)).Trim(' ', '\n', '\r'));
            return row;
        }
    }
}
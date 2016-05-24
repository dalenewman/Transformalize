
//using System;
//using Orchard.Templates.Services;
//using Pipeline.Configuration;
//using Pipeline.Contracts;
//using Pipeline.Transforms;

//namespace Pipeline.Web.Orchard.Impl {
//    public class OrchardRazorTransform : BaseTransform, ITransform {
//        private readonly ITemplateProcessor _processor;

//        private readonly Field[] _input;
//        private readonly bool _compiled;

//        public OrchardRazorTransform(IContext context, ITemplateProcessor processor) : base(context) {
//            _processor = processor;
//            _input = MultipleInput();

//            try {
//                _processor.Process(Context.Transform.Template, Context.Key);
//                _compiled = true;
//            } catch (Exception ex) {
//                Context.Warn(Context.Transform.Template.Replace("{", "{{").Replace("}", "}}"));
//                Context.Warn(ex.Message);
//                throw;
//            }
//        }

//        public IRow Transform(IRow row) {
//            if (_compiled)
//                row[Context.Field] = Context.Field.Convert(_processor.Process(Context.Transform.Template, Context.Key, null, row.ToFriendlyExpandoObject(_input)));
//            Increment();
//            return row;
//        }
//    }
//}
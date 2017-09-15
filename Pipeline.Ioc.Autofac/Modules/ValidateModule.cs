#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Autofac;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Validators;

namespace Transformalize.Ioc.Autofac.Modules {
    public class ValidateModule : Module {
        private readonly Process _process;

        public ValidateModule() { }

        public ValidateModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // return true or false, validators
            builder.Register((c, p) => new AnyValidator(p.Positional<IContext>(0))).Named<IValidate>("any");
            builder.Register((c, p) => new StartsWithValidator(p.Positional<IContext>(0))).Named<IValidate>("startswith");
            builder.Register((c, p) => new EndsWithValidator(p.Positional<IContext>(0))).Named<IValidate>("endswith");
            builder.Register((c, p) => new InValidator(p.Positional<IContext>(0))).Named<IValidate>("in");
            builder.Register((c, p) => new ContainsValidator(p.Positional<IContext>(0))).Named<IValidate>("contains");
            builder.Register((c, p) => new IsValidator(p.Positional<IContext>(0))).Named<IValidate>("is");
            builder.Register((c, p) => new EqualsValidator(p.Positional<IContext>(0))).Named<IValidate>("equals");
            builder.Register((c, p) => new EmptyValidator(p.Positional<IContext>(0))).Named<IValidate>("empty");
            builder.Register((c, p) => new DefaultValidator(p.Positional<IContext>(0))).Named<IValidate>("default");
            builder.Register((c, p) => new NumericValidator(p.Positional<IContext>(0))).Named<IValidate>("numeric");
            builder.Register((c, p) => new MatchValidator(p.Positional<IContext>(0))).Named<IValidate>("matches");
            builder.Register((c, p) => new RequiredValidator(p.Positional<IContext>(0))).Named<IValidate>("required");

            //builder.Register<IValidate>((c, p) => {
            //    var context = p.Positional<IContext>(0);
            //    if (c.ResolveNamed<IHost>("cs").Start()) {
            //        return new CsharpTransform(context);
            //    }
            //    context.Error("Unable to register csharp transform");
            //    return new NullTransform(context);
            //}).Named<IValidate>("cs");
            //builder.Register((c, p) => c.ResolveNamed<IValidate>("cs", p)).Named<IValidate>("csharp");


            //builder.Register<IValidate((c, p) => {
            //    var context = p.Positional<IContext>(0);
            //    switch (context.Field.Engine) {
            //        case "jint":
            //            return new JintTransform(context, c.Resolve<IReader>());
            //        default:
            //            return new JavascriptTransform(new ChakraCoreJsEngineFactory(), context, c.Resolve<IReader>());
            //    }
            //}).Named<IValidate>("js");
            //builder.Register((c, p) => c.ResolveNamed<IValidate>("js", p)).Named<IValidate>("javascript");

        }

    }
}
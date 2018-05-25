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
using CompareValidator = Transformalize.Validators.CompareValidator;
using RegularExpressionValidator = Transformalize.Validators.RegularExpressionValidator;

namespace Pipeline.Web.Orchard.Modules {
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
            builder.Register((c, p) => new AllValidator(p.Positional<IContext>(0))).Named<IValidate>("all");
            builder.Register((c, p) => new StartsWithValidator(p.Positional<IContext>(0))).Named<IValidate>("startswith");
            builder.Register((c, p) => new EndsWithValidator(p.Positional<IContext>(0))).Named<IValidate>("endswith");
            builder.Register((c, p) => new MapValidator(p.Positional<IContext>(0), inMap: true)).Named<IValidate>("in");
            builder.Register((c, p) => new MapValidator(p.Positional<IContext>(0), inMap: false)).Named<IValidate>("notin");
            builder.Register((c, p) => new ContainsValidator(p.Positional<IContext>(0))).Named<IValidate>("contains");
            builder.Register((c, p) => new IsValidator(p.Positional<IContext>(0))).Named<IValidate>("is");
            builder.Register((c, p) => new EqualsValidator(p.Positional<IContext>(0))).Named<IValidate>("equals");
            builder.Register((c, p) => new EmptyValidator(p.Positional<IContext>(0))).Named<IValidate>("empty");
            builder.Register((c, p) => new DefaultValidator(p.Positional<IContext>(0))).Named<IValidate>("default");
            builder.Register((c, p) => new NumericValidator(p.Positional<IContext>(0))).Named<IValidate>("numeric");
            builder.Register((c, p) => new MatchValidator(p.Positional<IContext>(0))).Named<IValidate>("matches");
            builder.Register((c, p) => new RequiredValidator(p.Positional<IContext>(0))).Named<IValidate>("required");
            builder.Register((c, p) => new MapValidator(p.Positional<IContext>(0), inMap: true)).Named<IValidate>("map");
            builder.Register((c, p) => new LengthValidator(p.Positional<IContext>(0))).Named<IValidate>("length");
            builder.Register((c, p) => new CompareValidator(p.Positional<IContext>(0), "min")).Named<IValidate>("min");
            builder.Register((c, p) => new CompareValidator(p.Positional<IContext>(0), "max")).Named<IValidate>("max");
            builder.Register((c, p) => new RegularExpressionValidator(p.Positional<IContext>(0), "^[a-zA-Z0-9]*$", "must be alphanumeric")).Named<IValidate>("alphanum");


        }

    }
}
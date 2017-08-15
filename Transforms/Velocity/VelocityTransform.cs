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
using System.IO;
using System.Text;
using NVelocity;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Transforms;
using System.Linq;
using Cfg.Net.Contracts;
using Cfg.Net.Loggers;

namespace Transformalize.Transform.Velocity {

    public class VelocityTransform : BaseTransform {

        private readonly Field[] _input;
        private readonly string _templateName;

        public VelocityTransform(IContext context, IReader reader) : base(context, context.Field.Type) {

            if (IsMissing(context.Transform.Template)) {
                return;
            }

            VelocityInitializer.Init();

            var fileBasedTemplate = context.Process.Templates.FirstOrDefault(t => t.Name == context.Transform.Template);

            if (fileBasedTemplate != null) {
                var memoryLogger = new MemoryLogger();
                context.Transform.Template = reader.Read(fileBasedTemplate.File, null, memoryLogger);
                if (memoryLogger.Errors().Any()) {
                    foreach (var error in memoryLogger.Errors()) {
                        context.Error(error);
                    }
                }
            }

            _input = MultipleInput();
            _templateName = Context.Field.Alias + " Template";
        }

        public override IRow Transform(IRow row) {

            var context = new VelocityContext();
            foreach (var field in _input) {
                context.Put(field.Alias, row[field]);
            }

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb)) {
                NVelocity.App.Velocity.Evaluate(context, sw, _templateName, Context.Transform.Template);
                sw.Flush();
            }

            sb.Trim(' ', '\n', '\r');
            row[Context.Field] = Context.Field.Convert(sb.ToString());

            Increment();
            return row;
        }

    }
}
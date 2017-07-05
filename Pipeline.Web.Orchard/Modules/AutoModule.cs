#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Web;
using Autofac;
using Orchard;
using Orchard.FileSystems.AppData;
using Orchard.Logging;
using Orchard.Templates.Services;
using Orchard.UI.Notify;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Pipeline.Web.Orchard.Impl;
using Transformalize;
using Module = Autofac.Module;

namespace Pipeline.Web.Orchard.Modules {
    public class AutoModule : Module {

        public ILogger Logger { get; set; }

        public AutoModule() {
            Logger = NullLogger.Instance;
        }

        protected override void Load(ContainerBuilder builder) {

            var logger = new OrchardLogger();
            var context = new PipelineContext(logger, new Process { Name = "OrchardCMS" });

            builder.Register(c => new RunTimeDataReader(logger, c.Resolve<IAppDataFolder>(), c.Resolve<ITemplateProcessor>(), c.Resolve<INotifier>())).As<IRunTimeRun>();
            builder.Register(c => new CachingRunTimeSchemaReader(new RunTimeSchemaReader(context, c.Resolve<IAppDataFolder>(), c.Resolve<ITemplateProcessor>(), c.Resolve<INotifier>()))).As<IRunTimeSchemaReader>();
            builder.Register(c => new SchemaHelper(context, c.Resolve<IRunTimeSchemaReader>())).As<ISchemaHelper>();
            builder.Register(c => new RunTimeExecuter(
                    context, 
                    c.Resolve<IAppDataFolder>(), 
                    new RazorReportTemplateProcessor(new RazorReportCompiler(), c.Resolve<HttpContextBase>(), c.Resolve<IOrchardServices>()), 
                    c.Resolve<INotifier>()
                )
            ).As<IRunTimeExecute>();

        }

    }
}

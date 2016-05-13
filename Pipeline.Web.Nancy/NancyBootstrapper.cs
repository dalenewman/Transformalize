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

using System.Diagnostics;
using Autofac;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using Pipeline.Contracts;
using Pipeline.Ioc.Autofac;
using Pipeline.Ioc.Autofac.Modules;

namespace Pipeline.Web.Nancy {

    public class NancyBootstrapper : AutofacNancyBootstrapper {
        private readonly IContext _context;

        public NancyBootstrapper(IContext context) {
            _context = context;
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer) {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_context.Logger).As<IPipelineLogger>();
            builder.RegisterInstance(_context).As<IContext>();
            builder.Register(ctx => new RunTimeDataReader(_context.Logger)).As<IRunTimeRun>();
            builder.Register(c => {
                var sw = new Stopwatch();
                sw.Start();
                return sw;
            }).As<Stopwatch>();
            builder.Update(existingContainer.ComponentRegistry);
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context) {
            var format = context.Request.Query["format"] ?? "json";
            var builder = new ContainerBuilder();
            builder.RegisterModule(new UnloadedRootModule("Shorthand.xml", format));

            builder.Update(container.ComponentRegistry);
        }
    }
}

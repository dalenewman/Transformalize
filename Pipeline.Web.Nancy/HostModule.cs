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

using System;
using Autofac;
using Cfg.Net.Ext;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Web.Nancy {
    public class HostModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder.Register<IContext>(c => new PipelineContext(c.Resolve<IPipelineLogger>(),new Process { Name = "Service", Key = "Service" }.WithDefaults())).As<IContext>();

            // for things to be available in nancy, inject into bootstrapper
            builder.Register<INancyBootstrapper>(ctx => new NancyBootstrapper(
                ctx.Resolve<IContext>()
            )).As<INancyBootstrapper>().InstancePerLifetimeScope();

            builder.Register((ctx, p) => {
                var settings = new HostConfiguration();
                var context = ctx.Resolve<IContext>();
                settings.UnhandledExceptionCallback = ex => {
                    context.Debug(() => ex.Message);
                };
                settings.UrlReservations.CreateAutomatically = true;
                return new NancyHost(ctx.Resolve<INancyBootstrapper>(), settings, new Uri($"http://localhost:{p.Named<int>("port")}"));
            }).As<NancyHost>();

            builder.Register<IHost>((ctx, p) => {
                var port = p.Named<int>("port");
                return new Host(port, ctx.Resolve<IContext>(), ctx.Resolve<NancyHost>(new NamedParameter("port", port)));
            }).As<IHost>();

        }
    }
}

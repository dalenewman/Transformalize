#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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

using System;
using Autofac;
using Common.Logging;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Extensions;
using Pipeline.Logging.NLog;
using Pipeline.Scheduler.Quartz;
using Quartz.Spi;

namespace Pipeline.Command {

    public class ScheduleModule : Module {
        private readonly Options _options;

        public ScheduleModule(Options options) {
            _options = options;
        }

        protected override void Load(ContainerBuilder builder) {

            _options.Arrangement = _options.ArrangementWithMode();

            builder.Register<IPipelineLogger>(c => new NLogPipelineLogger(_options.Arrangement)).As<IPipelineLogger>().SingleInstance();
            builder.Register<IContext>(c => new PipelineContext(c.Resolve<IPipelineLogger>())).As<IContext>();

            // for now scheduler
            builder.Register(c => new RunTimeSchemaReader(c.Resolve<IContext>())).As<IRunTimeSchemaReader>();
            builder.Register<ISchemaHelper>(ctx => new SchemaHelper(ctx.Resolve<IContext>(), ctx.Resolve<IRunTimeSchemaReader>())).As<ISchemaHelper>();

            // for quartz scheduler
            builder.RegisterType<QuartzJobFactory>().As<IJobFactory>().SingleInstance();
            builder.Register<ILoggerFactoryAdapter>((ctx => new QuartzLogAdaptor(ctx.Resolve<IContext>(), Scheduler.Quartz.Utility.ConvertLevel(ctx.Resolve<IContext>().LogLevel), true, true, false, "o"))).As<ILoggerFactoryAdapter>();

            builder.Register<IScheduler>((ctx, p) => {
                if (string.IsNullOrEmpty(_options.Schedule) || _options.Mode != null && _options.Mode.In("init", "check")) {
                    return new NowScheduler(_options, ctx.Resolve<ISchemaHelper>());
                }
                return new QuartzCronScheduler(_options, ctx.Resolve<IJobFactory>(), ctx.Resolve<ILoggerFactoryAdapter>());
            }).As<IScheduler>();

        }

    }
}

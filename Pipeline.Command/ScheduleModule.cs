#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Common.Logging;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Desktop.Transforms;
using Pipeline.Extensions;
using Pipeline.Logging.NLog;
using Pipeline.Scheduler.Quartz;
using Quartz.Spi;
using Environment = System.Environment;

namespace Pipeline.Command {

    public class ScheduleModule : Module {
        private readonly Options _options;

        public ScheduleModule(Options options) {
            _options = options;
        }

        protected override void Load(ContainerBuilder builder) {

            _options.Arrangement = _options.ArrangementWithMode();

            builder.Register<IPipelineLogger>(c => new NLogPipelineLogger(SlugifyTransform.Slugify(_options.Arrangement))).As<IPipelineLogger>().SingleInstance();
            builder.Register<IContext>(c => new PipelineContext(c.Resolve<IPipelineLogger>())).As<IContext>();

            // for now scheduler
            builder.Register(c => new RunTimeSchemaReader(c.Resolve<IContext>())).As<IRunTimeSchemaReader>();
            builder.Register<ISchemaHelper>(ctx => new SchemaHelper(ctx.Resolve<IContext>(), ctx.Resolve<IRunTimeSchemaReader>())).As<ISchemaHelper>();

            // for quartz scheduler
            builder.Register<ILoggerFactoryAdapter>((ctx => new QuartzLogAdaptor(ctx.Resolve<IContext>(), Scheduler.Quartz.Utility.ConvertLevel(ctx.Resolve<IContext>().LogLevel), true, true, false, "o"))).As<ILoggerFactoryAdapter>();
            builder.RegisterType<QuartzJobFactory>().As<IJobFactory>().SingleInstance();

            builder.Register<IScheduler>((ctx, p) => {
                if (string.IsNullOrEmpty(_options.Schedule) || _options.Mode != null && _options.Mode.In("init", "check")) {
                    return new NowScheduler(_options, ctx.Resolve<ISchemaHelper>());
                }

                if (_options.Schedule == "internal") {
                    var process = ProcessFactory.Create(_options.Arrangement, _options.Shorthand);
                    if (process.Errors().Any())
                    {
                        Console.Error.WriteLine("In order for an internal schedule to work, the arrangement passed in must be valid!");
                        foreach (var error in process.Errors())
                        {
                            Console.Error.WriteLine(error);
                        }
                        Environment.Exit(1);
                    }
                    return new InternalQuartzCronScheduler(_options, process.Schedule, ctx.Resolve<IJobFactory>(), ctx.Resolve<ILoggerFactoryAdapter>());
                }


                return new QuartzCronScheduler(_options, ctx.Resolve<IJobFactory>(), ctx.Resolve<ILoggerFactoryAdapter>());
            }).As<IScheduler>();

        }

    }
}

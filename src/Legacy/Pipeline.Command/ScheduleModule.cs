#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Microsoft.Extensions.Logging;
using Quartz.Spi;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Impl;
using Transformalize.Logging.NLog;
using Transformalize.Scheduler.Quartz;
using Transformalize.Transforms.Globalization;
using Environment = System.Environment;

namespace Transformalize.Command {

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
            builder.Register(c => new RunTimeSchemaReader(c.Resolve<IContext>(), _options.PlaceHolderStyle)).As<IRunTimeSchemaReader>();
            builder.Register<ISchemaHelper>(ctx => new SchemaHelper(ctx.Resolve<IContext>(), ctx.Resolve<IRunTimeSchemaReader>())).As<ISchemaHelper>();

            // for quartz scheduler
            builder.Register<ILoggerFactory>(ctx => new QuartzLogFactory(ctx.Resolve<IContext>(), ctx.Resolve<IContext>().LogLevel)).As<ILoggerFactory>();
            builder.Register(ctx => new QuartzJobFactory(_options, ctx.Resolve<IPipelineLogger>())).As<IJobFactory>().SingleInstance();

            builder.Register<IScheduler>((ctx, p) => {
                if (string.IsNullOrEmpty(_options.Schedule) || _options.Mode != null && _options.Mode.In("init", "schema")) {
                    return new NowScheduler(_options, ctx.Resolve<ISchemaHelper>(), ctx.Resolve<IPipelineLogger>());
                }

                if (_options.Schedule == "internal") {
                    var process = ProcessFactory.Create(_options, new Dictionary<string, string>());
                    if (process.Errors().Any()) {
                        Console.Error.WriteLine("In order for an internal schedule to work, the arrangement passed in must be valid!");
                        foreach (var error in process.Errors()) {
                            Console.Error.WriteLine(error);
                        }
                        Environment.Exit(1);
                    }
                    return new QuartzCronSchedulerViaInternalSchedule(_options, process.Schedule, ctx.Resolve<IJobFactory>(), ctx.Resolve<ILoggerFactory>());
                }

                return new QuartzCronSchedulerViaCommandLine(_options, ctx.Resolve<IJobFactory>(), ctx.Resolve<ILoggerFactory>());
            }).As<IScheduler>();

        }

    }
}

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
using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Transformalize.Command {
    public class QuartzCronSchedulerViaCommandLine : Contracts.IScheduler {
        readonly Quartz.IScheduler _scheduler;
        private readonly Options _options;
        private readonly ILog _logger;

        public QuartzCronSchedulerViaCommandLine(Options options, IJobFactory jobFactory, ILoggerFactoryAdapter loggerFactory) {
            _options = options;
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.JobFactory = jobFactory;

            LogManager.Adapter = loggerFactory;
            _logger = LogManager.GetLogger("Quartz.Net");
        }

        public void Start() {

            _logger.Info($"Starting Scheduler: {_options.Schedule}");
            _scheduler.Start();

            var job = JobBuilder.Create<ScheduleExecutor>()
                .WithIdentity("Job", "TFL")
                .WithDescription("Scheduled TFL Job")
                .StoreDurably(false)
                .RequestRecovery(false)
                .UsingJobData("Cfg", _options.Arrangement)
                .UsingJobData("Mode", _options.Mode)
                .UsingJobData("Schedule", string.Empty)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("Trigger", "TFL")
                .StartNow()
                .WithCronSchedule(_options.Schedule, x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            _scheduler.ScheduleJob(job, trigger);
        }

        public void Stop() {
            if (!_scheduler.IsStarted)
                return;

            _logger.Info("Stopping Scheduler...");
            _scheduler.Shutdown(true);
        }

    }
}

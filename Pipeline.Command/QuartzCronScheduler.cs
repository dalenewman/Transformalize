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
using System.Runtime.InteropServices;
using Common.Logging;
using Common.Logging.Configuration;
using Pipeline.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Pipeline.Command {

    public class InternalQuartzCronScheduler : Contracts.IScheduler {
        readonly Quartz.IScheduler _scheduler;
        private readonly Options _options;
        private readonly ILog _logger;
        private readonly List<Schedule> _schedule;

        public InternalQuartzCronScheduler(Options options, List<Schedule> schedule, IJobFactory jobFactory, ILoggerFactoryAdapter loggerFactory) {
            _options = options;
            _schedule = schedule;
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.JobFactory = jobFactory;

            LogManager.Adapter = loggerFactory;
            _logger = LogManager.GetLogger("Quartz.Net");
        }

        public void Start() {
            _logger.Info($"Starting Scheduler: {_options.Schedule}");
            _scheduler.Start();

            foreach (var schedule in _schedule) {

                if (_options.Mode != "default" && schedule.Mode != _options.Mode) {
                    Console.Error.WriteLine($"Note: The internal schedule's mode of {schedule.Mode} trumps your command line mode of {_options.Mode}.");
                }

                var job = JobBuilder.Create<RunTimeExecutor>()
                    .WithIdentity(schedule.Name + " Job", "TFL")
                    .StoreDurably(false)
                    .RequestRecovery(false)
                    .WithDescription("Transformalize Quartz.Net Job")
                    .UsingJobData("Cfg", _options.Arrangement)
                    .UsingJobData("Shorthand", _options.Shorthand)
                    .UsingJobData("CommandLine.Mode", schedule.Mode)
                    .UsingJobData("CommandLine.Format", _options.Format)
                    .UsingJobData("Schedule", schedule.Cron)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(schedule.Name + " Trigger", "TFL")
                    .StartNow()
                    .WithCronSchedule(schedule.Cron, x => x.WithMisfireHandlingInstructionIgnoreMisfires())
                    .Build();

                _scheduler.ScheduleJob(job, trigger);

            }



        }

        public void Stop() {
            if (!_scheduler.IsStarted)
                return;

            _logger.Info("Stopping Scheduler...");
            _scheduler.Shutdown(true);
        }
    }

    public class QuartzCronScheduler : Contracts.IScheduler {
        readonly Quartz.IScheduler _scheduler;
        private readonly Options _options;
        private readonly ILog _logger;

        public QuartzCronScheduler(Options options, IJobFactory jobFactory, ILoggerFactoryAdapter loggerFactory) {
            _options = options;
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.JobFactory = jobFactory;

            LogManager.Adapter = loggerFactory;
            _logger = LogManager.GetLogger("Quartz.Net");
        }

        public void Start() {

            _logger.Info($"Starting Scheduler: {_options.Schedule}");
            _scheduler.Start();

            var job = JobBuilder.Create<RunTimeExecutor>()
                .WithIdentity("Job", "TFL")
                .StoreDurably(false)
                .RequestRecovery(false)
                .WithDescription("Transformalize Quartz.Net Job")
                .UsingJobData("Cfg", _options.Arrangement)
                .UsingJobData("Shorthand", _options.Shorthand)
                .UsingJobData("CommandLine.Mode", _options.Mode)
                .UsingJobData("CommandLine.Format", _options.Format)
                .UsingJobData("Schedule", _options.Schedule)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("Trigger", "TFL")
                .StartNow()
                .WithCronSchedule(_options.Schedule, x => x.WithMisfireHandlingInstructionIgnoreMisfires())
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

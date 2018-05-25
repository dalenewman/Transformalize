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
using System;
using System.Collections.Generic;
using System.Xml;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Transformalize.Configuration;
using Humanizer;

namespace Transformalize.Command {

    public class QuartzCronSchedulerViaInternalSchedule : Contracts.IScheduler {
        readonly Quartz.IScheduler _scheduler;
        private readonly Options _options;
        private readonly ILog _logger;
        private readonly List<Schedule> _schedule;

        public QuartzCronSchedulerViaInternalSchedule(Options options, List<Schedule> schedule, IJobFactory jobFactory, ILoggerFactoryAdapter loggerFactory) {
            _options = options;
            _schedule = schedule;
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.JobFactory = jobFactory;

            LogManager.Adapter = loggerFactory;
            _logger = LogManager.GetLogger("Quartz.Net");
        }

        public void Start() {

            foreach (var schedule in _schedule) {

                if (_options.Mode != "default" && schedule.Mode != _options.Mode) {
                    Console.Error.WriteLine($"Note: The internal schedule's mode of {schedule.Mode} trumps your command line mode of {_options.Mode}.");
                }

                _logger.Info($"Schedule {schedule.Name} set for {schedule.Cron} in {schedule.Mode} mode.");

                var job = JobBuilder.Create<ScheduleExecutor>()
                    .WithIdentity(schedule.Name, "TFL")
                    .WithDescription($"Scheduled TFL Job: {schedule.Name}")
                    .StoreDurably(false)
                    .RequestRecovery(false)
                    .UsingJobData("Cfg", _options.Arrangement)
                    .UsingJobData("Mode", schedule.Mode)
                    .UsingJobData("Schedule", schedule.Name)
                    .Build();

                ITrigger trigger;

                switch (schedule.MisFire) {
                    case "ignore":
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(schedule.Name.Titleize() + " Trigger", "TFL")
                            .StartNow()
                            .WithCronSchedule(schedule.Cron, x => x
                                .WithMisfireHandlingInstructionIgnoreMisfires()
                                .InTimeZone(schedule.TimeZone == Constants.DefaultSetting ? TimeZoneInfo.Local : TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZone))
                            ).Build();

                        break;
                    case "fire":
                    case "fireandproceed":
                        trigger = TriggerBuilder.Create()
                           .WithIdentity(schedule.Name.Titleize() + " Trigger", "TFL")
                            .StartNow()
                            .WithCronSchedule(schedule.Cron, x => x
                                .WithMisfireHandlingInstructionFireAndProceed()
                                .InTimeZone(schedule.TimeZone == Constants.DefaultSetting ? TimeZoneInfo.Local : TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZone))
                            ).Build();

                        break;
                    default:
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(schedule.Name.Titleize() + " Trigger", "TFL")
                            .StartNow()
                            .WithCronSchedule(schedule.Cron, x => x
                                .WithMisfireHandlingInstructionDoNothing()
                                .InTimeZone(schedule.TimeZone == Constants.DefaultSetting ? TimeZoneInfo.Local : TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZone))
                            ).Build();
                        break;
                }

                _scheduler.ScheduleJob(job, trigger);

            }
            _scheduler.Start();
        }

        public void Stop() {
            if (!_scheduler.IsStarted)
                return;

            _logger.Info("Stopping Scheduler...");
            _scheduler.Shutdown(true);
        }
    }
}
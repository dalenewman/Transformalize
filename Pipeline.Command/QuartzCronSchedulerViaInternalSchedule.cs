using System;
using System.Collections.Generic;
using Common.Logging;
using Pipeline.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Pipeline.Command {

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
            _logger.Info($"Starting Scheduler: {_options.Schedule}");
            _scheduler.Start();

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
                    .UsingJobData("Shorthand", _options.Shorthand)
                    .UsingJobData("Mode", schedule.Mode)
                    .UsingJobData("Schedule", schedule.Name)
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
}
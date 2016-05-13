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
using Pipeline.Contracts;
using Pipeline.Scheduler.Quartz;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Pipeline.Command {

    public class QuartzNowScheduler : Contracts.IScheduler {

        readonly Quartz.IScheduler _scheduler;
        readonly IContext _context;
        private readonly Options _options;

        public QuartzNowScheduler(Options options, IContext context, IJobFactory jobFactory) {
            Common.Logging.LogManager.Adapter = new QuartzLogAdaptor(context, Scheduler.Quartz.Utility.ConvertLevel(context.LogLevel), true, true, false, "o");
            _context = context;
            _options = options;
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.JobFactory = jobFactory;
        }

        public void Start() {
            _context.Info("Starting Scheduler...");
            _scheduler.Start();

            var job = JobBuilder.Create<RunTimeExecutor>()
                .WithIdentity("Job", "Pipeline.Net")
                .StoreDurably(false)
                .RequestRecovery(false)
                .WithDescription("Pipeline.Net Quartz Job")
                .UsingJobData("cfg",_options.Configuration)
                .UsingJobData("shorthand",_options.Shorthand)
                .UsingJobData("mode", _options.Mode)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("Tgr", "Pipeline.Net")
                .StartNow()
                .Build();

            _scheduler.ScheduleJob(job, trigger);
            _scheduler.TriggerJob(job.Key);
        }

        public void Stop() {
            if (!_scheduler.IsStarted)
                return;

            _context.Info("Stopping Scheduler...");
            _scheduler.Shutdown(true);
        }

    }
}

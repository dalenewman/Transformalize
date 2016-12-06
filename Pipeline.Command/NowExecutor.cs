using System;
using System.Collections.Generic;
using Quartz;

namespace Transformalize.Command {
    [DisallowConcurrentExecution]
    public class NowExecutor : BaseExecutor, IJob, IDisposable {

        public string Shorthand { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public NowExecutor(string cfg, string shorthand, string mode, string format) : base(cfg, mode, format) {
            Shorthand = shorthand;
        }

        /// <summary>
        /// This is the method Quartz.NET will use
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context) {
            Execute(Cfg, Shorthand, Parameters);
        }

        public void Dispose() {
            // shouldn't be anything to dispose
        }
    }
}
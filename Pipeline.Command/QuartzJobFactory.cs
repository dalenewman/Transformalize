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
using System.Diagnostics;
using Quartz;
using Quartz.Spi;
using IScheduler = Quartz.IScheduler;

namespace Pipeline.Command {

    public class QuartzJobFactory : IJobFactory {

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) {
            return new RunTimeExecutor();
        }

        public void ReturnJob(IJob job) {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }

}

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
using Transformalize.Contracts;
using System.Threading;
using System;

namespace Transformalize.Transforms.System {
   public class LogTimerTransform : BaseTransform {

      private static readonly object _logLock = new object();
      private bool _log = false;

      public LogTimerTransform(IContext context = null) : base(context, "null") {
         if (IsMissingContext()) {
            return;
         }
         var startTimeSpan = TimeSpan.Zero;
         var periodTimeSpan = TimeSpan.FromMilliseconds(Convert.ToDouble(Context.Entity.LogInterval));
         var timer = new Timer((e) => {
            lock (_logLock) {
               _log = true;
            }
         }, null, startTimeSpan, periodTimeSpan);

      }

      public override IRow Operate(IRow row) {

         if (_log) {
            lock (_logLock) {
               Context.Info($"{Context.Entity.RowNumber} rows transformed.");
               _log = false;
            }
         }

         return row;
      }


   }
}
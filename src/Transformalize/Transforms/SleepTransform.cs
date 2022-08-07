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
using System.Collections.Generic;
using Transformalize.Contracts;
using Action = global::System.Action;


namespace Transformalize.Transforms {
   public class SleepTransform : BaseTransform {
      private readonly Action _sleep;
      private readonly global::System.TimeSpan _delay;
      private readonly string _delayString;
      public SleepTransform(IContext context = null) : base(context, null) {
         if (IsMissingContext()) {
            return;
         }

         _delay = global::System.TimeSpan.FromMilliseconds(Context.Operation.Time);
         _delayString = _delay.ToString();

#if NETS10
         _sleep = () => global::System.Threading.Tasks.Task.Delay(_delay).Wait();
#else
         _sleep = () => global::System.Threading.Thread.Sleep(_delay);
#endif
      }

      public override IRow Operate(IRow row) {
         Context.Info("Sleeping for {0}", _delayString);
         _sleep();
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         var sleep = new OperationSignature("sleep") { Parameters = new List<OperationParameter>(1) { new OperationParameter("time") } };
         var wait = new OperationSignature("wait") { Parameters = new List<OperationParameter>(1) { new OperationParameter("time") } };
         return new OperationSignature[] { sleep, wait };
      }

   }

}

#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Impl {

   /// <summary>
   /// Things that every output needs
   /// </summary>
   public abstract class BaseOutputController : IOutputController {

      public OutputContext Context { get; set; }
      public IAction Initializer { get; set; }
      public IInputProvider InputProvider { get; set; }
      public IOutputProvider OutputProvider { get; set; }

      protected BaseOutputController(
          OutputContext context,
          IAction initializer,
          IInputProvider inputProvider,
          IOutputProvider outputProvider) {
         Context = context;
         Initializer = initializer;
         InputProvider = inputProvider;
         OutputProvider = outputProvider;
      }

      public virtual ActionResponse Initialize() {
         Context.Debug(() => $"Initializing with {Initializer.GetType().Name}");
         return Initializer.Execute();
      }

      /// <summary>
      /// Implementation should over-ride Start, but still run base.Start() to set
      /// Context.Entity.MaxVersion and Context.Entity.MinVersion.  In addition, the 
      /// implementation should:
      /// 
      /// * query and set Context.Entity.BatchId (the max. TflBatchId) 
      /// * query and set Context.Entity.Identity (the max. TflKey)
      /// * query if output has any records and use in conjunction with MinVersion determine Content.Entity.IsFirstRun (MinVersion == null && outputCount == 0)
      /// </summary>
      public virtual void Start() {
         Context.Debug(() => "Starting");
         Context.Entity.MaxVersion = InputProvider.GetMaxVersion();
         Context.Entity.MinVersion = OutputProvider.GetMaxVersion();
         Context.Entity.BatchId = OutputProvider.GetNextTflBatchId();
         Context.Entity.Identity = OutputProvider.GetMaxTflKey();
      }

      /// <summary>
      /// Implementation may optionally over-ride End
      /// </summary>
      public virtual void End() {
         Context.Debug(() => "Ending");
      }

      public virtual Task<ActionResponse> InitializeAsync(CancellationToken cancellationToken = default) => Task.FromResult(Initialize());
      public virtual Task StartAsync(CancellationToken cancellationToken = default) {
         Start();
         return Task.CompletedTask;
      }
      public virtual Task EndAsync(CancellationToken cancellationToken = default) {
         End();
         return Task.CompletedTask;
      }
   }
}
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Transformalize.Contracts;

namespace Transformalize.Impl {

   public class ProcessController : IProcessController {

      private readonly IEnumerable<IPipeline> _pipelines;
      private readonly IContext _context;
      public List<IAction> PreActions { get; } = new List<IAction>();
      public List<IAction> PostActions { get; } = new List<IAction>();
      private Stopwatch _stopwatch = new Stopwatch();

      public ProcessController(
          IEnumerable<IPipeline> pipelines,
          IContext context
      ) {
         _stopwatch.Start();
         _pipelines = pipelines;
         _context = context;
      }

      private bool PreExecute() {
         foreach (var action in PreActions) {
            _context.Debug(() => $"Pre-Executing {action.GetType().Name}");
            if (!MayContinue(action.Execute())) {
               return false;
            }
         }
         return true;
      }

      public void Execute() {
         if (PreExecute()) {
            foreach (var entity in _pipelines) {
               _context.Debug(() => $"Initializing {entity.GetType().Name}");
               if (!MayContinue(entity.Initialize())) {
                  return;
               }
            }
            foreach (var entity in _pipelines) {
               _context.Debug(() => $"Executing {entity.GetType().Name}");
               entity.Execute();
            }
            PostExecute();
         } else {
            _context.Error("Pre-Execute failed!");
         }
      }


#if ASYNC
      public async Task ExecuteAsync() {

         Task task = Task.Run(() => Execute());
         try {
            await task.ConfigureAwait(false);
         } catch (Exception ex) {
            _context.Error(ex, ex.Message);
            if (task != null && task.Exception != null) {
               if (task.Exception.Message != ex.Message) {
                  _context.Error(task.Exception, task.Exception.Message);
               }
               if (task.Exception.InnerException != null && task.Exception.InnerException.Message != task.Exception.Message && task.Exception.InnerException.Message != ex.Message) {
                  _context.Error(task.Exception, task.Exception.InnerException.Message);
               }
            }
         }
      }
#else
      public Task ExecuteAsync() {
         throw new NotImplementedException("Must be using .NET 4.6.2 or .NET Standard 2.0 library for this method to work");
      }
#endif

      private void PostExecute() {
         foreach (var action in PostActions) {
            _context.Debug(() => $"Post-Executing {action.GetType().Name}");
            if (!MayContinue(action.Execute())) {
               return;
            }
         }
      }

      private bool MayContinue(ActionResponse response) {
         if (response.Code == 200) {
            if (response.Action.Type != "internal") {
               if (response.Action.Description == string.Empty) {
                  _context.Info($"Successfully ran action {response.Action.Type}.");
               } else {
                  _context.Info($"Successfully ran action {response.Action.Type}: {response.Action.Description}.");
               }
            }
            if (response.Message != string.Empty) {
               _context.Debug(() => response.Message);
            }
            return true;
         }

         var errorMode = response.Action.ToErrorMode();

         if (errorMode == ErrorMode.Ignore) {
            return true;
         }

         if (errorMode == ErrorMode.Continue) {
            _context.Error("Continue: " + response.Message);
            return true;
         }

         if (errorMode == ErrorMode.Default || errorMode == ErrorMode.Abort) {
            _context.Error("Abort: " + response.Message);
            return false;
         }

         if (errorMode == ErrorMode.Exception) {
            _context.Error("Exception: " + response.Message);
            throw new Exception(response.Message);
         }

         return false;
      }

      public IEnumerable<IRow> Read() {
         foreach (var pl in _pipelines) {
            foreach (var row in pl.Read()) {
               yield return row;
            }
         };
      }

      public void Dispose() {
         PreActions.Clear();
         PostActions.Clear();
         foreach (var pipeline in _pipelines) {
            pipeline.Dispose();
         }
         _stopwatch.Stop();
         _context.Info($"Time elapsed: {_stopwatch.Elapsed}");
         _stopwatch = null;
      }
   }
}

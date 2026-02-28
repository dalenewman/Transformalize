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
using Autofac;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Containers.Autofac {

   public class PipelineAction : IAction {

      private readonly Action _action;
      private readonly string _cfg;
      private readonly IContext _context;

      public PipelineAction(IContext context, Action action, string cfg) {
         _action = action;
         _cfg = cfg;
         _context = context;
      }

      public ActionResponse Execute() {
         var response = new ActionResponse() { Action = _action };

         using(var outer = new ConfigurationContainer().CreateScope(_cfg, _context.Logger, null, _action.PlaceHolderStyle)){
            var process = outer.Resolve<Process>();
            foreach (var warning in process.Warnings()) {
               _context.Warn(warning);
            }
            if (process.Errors().Any()) {
               foreach (var error in process.Errors()) {
                  _context.Error(error);
               }
               response.Code = 500;
               response.Message = $"TFL Pipeline Action '{_cfg.Left(30)}'... has errors!";
               return response;
            } else {
               using (var inner = new Container().CreateScope(process, _context.Logger)) {
                  inner.Resolve<IProcessController>().Execute();
                  response.Code = System.Convert.ToInt32(process.Status);
                  response.Message = process.Message;
               }
            }
         }

         return response;
      }
   }
}
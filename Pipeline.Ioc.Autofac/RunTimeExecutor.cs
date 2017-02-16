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
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac.Modules;

namespace Transformalize.Ioc.Autofac {

    public class RunTimeExecutor : IRunTimeExecute {
        private readonly IContext _context;

        public RunTimeExecutor(IContext context) {
            _context = context;
        }

        public void Execute(Process process) {

            foreach (var warning in process.Warnings()) {
                _context.Warn(warning);
            }

            if (process.Errors().Any()) {
                foreach (var error in process.Errors()) {
                    _context.Error(error);
                }
                _context.Error("The configuration errors must be fixed before this job will run.");
                return;
            }

            using (var scope = DefaultContainer.Create(process, _context.Logger)) {
                try {
                    scope.Resolve<IProcessController>().Execute();
                } catch (Exception ex) {
                    _context.Error(ex.Message);
                }
            }
        }

        public void Execute(string cfg, string shorthand, Dictionary<string, string> parameters) {
            var builder = new ContainerBuilder();
            builder.RegisterCallback(new RootModule(shorthand).Configure);
            using (var scope = builder.Build().BeginLifetimeScope()) {
                var process = scope.Resolve<Process>(
                    new NamedParameter("cfg", cfg),
                    new NamedParameter("parameters", parameters)
                );
                Execute(process);
            }
        }

    }
}
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
using System.Diagnostics;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.Ado {

    public class AdoStarController : IOutputController {

        readonly Stopwatch _stopWatch;
        readonly OutputContext _context;
        readonly IAction _initializer;

        public AdoStarController(OutputContext context, IAction initializer) {
            _context = context;
            _initializer = initializer;
            _stopWatch = new Stopwatch();
        }

        public void Initialize() {
            _context.Debug(()=> $"Initializing with {_initializer.GetType().Name}");
            _initializer.Execute();
        }

        public void Start() {
            _stopWatch.Start();
        }

        public void End() {
            _stopWatch.Stop();
            _context.Info("Process Time: {0}", _stopWatch.Elapsed);
        }
    }
}
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

using NVelocity.Runtime;
using NVelocity.Runtime.Log;
using Transformalize.Contracts;
using LogLevel = NVelocity.Runtime.Log.LogLevel;

namespace Transformalize.Template.Velocity {
    public class VelocityLogSystem : ILogSystem {
        private readonly IContext _context;

        public VelocityLogSystem(IContext context) {
            _context = context;
        }

        public void Init(IRuntimeServices rs) {
        }

        public void LogVelocityMessage(LogLevel level, string message) {
            switch (level) {
                case LogLevel.Debug:
                    _context.Debug(() => message);
                    break;
                case LogLevel.Error:
                    _context.Error(message);
                    break;
                case LogLevel.Info:
                    _context.Info(message);
                    break;
                case LogLevel.Warn:
                    _context.Warn(message);
                    break;
            }
        }
    }
}

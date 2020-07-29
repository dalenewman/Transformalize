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

using Microsoft.Extensions.Logging;

namespace Transformalize.Scheduler.Quartz {

   public class QuartzLogFactory : ILoggerFactory {

      readonly Contracts.IContext _context;
      readonly Contracts.LogLevel _level;

      public QuartzLogFactory(Contracts.IContext context, Contracts.LogLevel level) {
         _context = context;
         _level = level;
      }

      public void AddProvider(ILoggerProvider provider) {
      }

      public ILogger CreateLogger(string categoryName) {
         return new QuartzLogger(_context, _level);
      }

      public void Dispose() {
         
      }
   }
}

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
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Transformalize.Logging;

namespace Transformalize.Scheduler.Quartz {
   public class QuartzLogger : ILogger {

      private readonly Contracts.IContext _context;
      private readonly BaseLogger _base;

      public QuartzLogger(Contracts.IContext context, Contracts.LogLevel level)  {
         _context = context;
         _base = new BaseLogger(level);
      }

      public IDisposable BeginScope<TState>(TState state) {
         return NullLoggerProvider.Instance;
      }

      public bool IsEnabled(LogLevel logLevel) {
         switch (logLevel) {
            case LogLevel.Trace:
               return false;
            case LogLevel.Debug:
               return _base.DebugEnabled;
            case LogLevel.Information:
               return _base.InfoEnabled;
            case LogLevel.Warning:
               return _base.WarnEnabled;
            case LogLevel.Error:
               return _base.ErrorEnabled;
            case LogLevel.Critical:
               return _base.ErrorEnabled;
            case LogLevel.None:
               return false;
            default:
               return true;
         }
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
         switch (logLevel) {
            case LogLevel.Debug:
               _context.Debug(()=>formatter(state,exception));
               break;
            case LogLevel.Error:
               if (exception == null) {
                  _context.Error(formatter(state,exception));
               } else {
                  _context.Error(exception, formatter(state, exception));
               }
               break;
            case LogLevel.Critical:
               if (exception == null) {
                  _context.Error(formatter(state, exception));
               } else {
                  _context.Error(exception, formatter(state, exception));
               }
               break;
            case LogLevel.Information:
               _context.Info(formatter(state, exception));
               break;
            case LogLevel.None:
               break;
            case LogLevel.Trace:
               _context.Debug(()=>formatter(state, exception));
               break;
            case LogLevel.Warning:
               _context.Warn(formatter(state, exception));
               break;
         }

      }


   }

}

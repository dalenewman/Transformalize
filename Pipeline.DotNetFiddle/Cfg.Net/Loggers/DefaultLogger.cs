#region license
// Cfg.Net
// Copyright 2015 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Cfg.Net.Loggers {
    internal sealed class DefaultLogger : ILogger {
        private readonly List<ILogger> _loggers = new List<ILogger>();
        private readonly MemoryLogger _memorylogger;

        public DefaultLogger(MemoryLogger memorylogger, ILogger logger) {
            _memorylogger = memorylogger;
            _loggers.Add(memorylogger);
            if (logger != null) {
                _loggers.Add(logger);
            }
        }

        public void Warn(string message, params object[] args) {
            for (int i = 0; i < _loggers.Count; i++) {
                _loggers[i].Warn(message, args);
            }
        }

        public void Error(string message, params object[] args) {
            for (int i = 0; i < _loggers.Count; i++) {
                _loggers[i].Error(message, args);
            }
        }

        public string[] Errors() {
            return _memorylogger.Errors();
        }

        public string[] Warnings() {
            return _memorylogger.Warnings();
        }
    }
}
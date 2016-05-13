#region license
// Transformalize
// Copyright 2013 Dale Newman
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
using System;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Logging {
    public class LogEntry {
        public PipelineContext Context { get; private set; }
        public DateTime Time { get; private set; }
        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; set; }

        public LogEntry(LogLevel level, PipelineContext context, string message, params object[] args) {
            Time = DateTime.UtcNow;
            Context = context;
            Level = level;
            Message = string.Format(message, args);
        }
    }
}
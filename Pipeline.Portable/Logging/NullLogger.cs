#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Logging {
    public class NullLogger : BaseLogger, IPipelineLogger {
        public NullLogger() : base(LogLevel.None) {}
        public void Debug(PipelineContext context, Func<string> lamda) {}
        public void Info(PipelineContext context, string message, params object[] args) {}
        public void Warn(PipelineContext context, string message, params object[] args) {}
        public void Error(PipelineContext context, string message, params object[] args) {}
        public void Error(PipelineContext context, Exception exception, string message, params object[] args) {}
        public void Clear() { }
    }
}
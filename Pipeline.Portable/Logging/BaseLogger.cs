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
using Pipeline.Contracts;

namespace Pipeline.Logging {
    public class BaseLogger {

        public LogLevel LogLevel { get; }

        public BaseLogger(LogLevel level) {
            LogLevel = level;
            var levels = GetLevels(level);
            DebugEnabled = levels[0];
            InfoEnabled = levels[1];
            WarnEnabled = levels[2];
            ErrorEnabled = levels[3];
        }

        public bool InfoEnabled { get; }

        public bool DebugEnabled { get; }

        public bool WarnEnabled { get; }

        public bool ErrorEnabled { get; }

        static bool[] GetLevels(LogLevel level) {
            switch (level) {
                case LogLevel.Debug:
                    return new[] { true, true, true, true };
                case LogLevel.Info:
                    return new[] { false, true, true, true };
                case LogLevel.Warn:
                    return new[] { false, false, true, true };
                case LogLevel.Error:
                    return new[] { false, false, false, true };
                case LogLevel.None:
                    return new[] { false, false, false, false };
                default:
                    goto case LogLevel.Info;
            }
        }
    }
}
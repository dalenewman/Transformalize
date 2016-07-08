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

using System;
using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Cfg.Net.Reader {
    public class ReTryingReader : IReader {
        private readonly IReader _reader;
        private readonly int _attempts;
        private readonly int _sleep;

        public ReTryingReader(IReader reader, int attempts, int sleep = 1000) {
            _reader = reader;
            _attempts = attempts;
            _sleep = sleep;
        }

        public string Read(string resource, IDictionary<string, string> parameters, ILogger logger) {

            if (string.IsNullOrEmpty(resource)) {
                logger.Error("Your configuration resource is null or empty.");
                return null;
            }

            for (var i = 0; i < _attempts; i++) {
                try {
                    var cfg = _reader.Read(resource, parameters, logger);
                    if (!string.IsNullOrEmpty(cfg)) {
                        return cfg;
                    }
                } catch (Exception ex) {
                    logger.Error(ex.Message);
                }
                logger.Warn($"Sleeping {_sleep}ms.");
                System.Threading.Thread.Sleep(_sleep);
            }
            return null;
        }
    }
}
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
    public class DefaultReader : IReader {
        private readonly IReader _fileReader;
        private readonly IReader _webReader;

        public DefaultReader(
            IReader fileReader,
            IReader webReader
            ) {
            _fileReader = fileReader;
            _webReader = webReader;
        }

        public string Read(string resource, IDictionary<string, string> parameters, ILogger logger) {

            if (string.IsNullOrEmpty(resource)) {
                logger.Error("Your configuration resource null or empty.");
                return null;
            }

            switch (resource[0]) {
                case '<':
                case '{':
                    return resource;
                default:
                    try {
                        return new Uri(resource).IsFile ? _fileReader.Read(resource, parameters, logger) : _webReader.Read(resource, parameters, logger);
                    } catch (Exception) {
                        return _fileReader.Read(resource, parameters, logger);
                    }
            }

        }
    }
}
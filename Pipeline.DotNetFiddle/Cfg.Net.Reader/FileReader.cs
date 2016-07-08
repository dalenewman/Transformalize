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
using System.IO;
using System.Linq;
using Cfg.Net.Contracts;

namespace Cfg.Net.Reader {
    public class FileReader : IReader {

        public string Read(string fileName, IDictionary<string, string> parameters, ILogger logger) {

            if (string.IsNullOrEmpty(fileName)) {
                logger.Error("Your configuration file name is null or empty.");
                return null;
            }

            var queryStringIndex = fileName.IndexOf('?');
            if (queryStringIndex > 0) {
                var newParameters = HttpUtility.ParseQueryString(fileName.Substring(queryStringIndex + 1));
                foreach (var pair in newParameters) {
                    parameters[pair.Key] = pair.Value;
                }
                fileName = fileName.Substring(0, queryStringIndex);
            }

            var lastPart = fileName.Split('\\').Last();
            var intersection = lastPart.Intersect(Path.GetInvalidFileNameChars()).ToArray();

            if (intersection.Any()) {
                logger.Error("Your configuration file name contains invalid characters: " + string.Join(", ", intersection) + ".");
                return null;
            }

            if (Path.HasExtension(fileName)) {
                if (!Path.IsPathRooted(fileName)) {
                    fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                }

                var fileInfo = new FileInfo(fileName);
                try {
                    return File.ReadAllText(fileInfo.FullName);
                } catch (Exception ex) {
                    logger.Error("Can not read file. {0}", ex.Message);
                    return null;
                }
            }

            logger.Error("Invalid file name: {0}.  File must have an extension (e.g. xml, json, etc)", fileName);
            return null;
        }
    }
}
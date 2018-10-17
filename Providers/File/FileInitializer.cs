#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2018 Dale Newman
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
#region license
#endregion

using System;
using System.IO;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.File {

    public class FileInitializer : IInitializer {

        private readonly OutputContext _context;

        public FileInitializer(OutputContext context) {
            _context = context;
        }

        public ActionResponse Execute() {

            _context.Warn("Initializing");

            var fileInfo = new FileInfo(_context.Connection.File);

            if (fileInfo.Directory != null && !fileInfo.Directory.Exists) {
                try {
                    fileInfo.Directory.Create();
                } catch (UnauthorizedAccessException ex) {
                    _context.Warn("Unable to create folder {0}", ex.Message);
                }
            }

            return new ActionResponse();
        }
    }
}
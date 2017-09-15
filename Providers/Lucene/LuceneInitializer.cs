#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.IO;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Lucene {
    public class LuceneInitializer : IInitializer {
        private readonly OutputContext _output;

        public LuceneInitializer(OutputContext output) {
            _output = output;
        }

        public ActionResponse Execute() {
            var response = new ActionResponse();
            _output.Info("Deleting lucene index at {0}.", _output.Connection.Folder);
            try {
                var directoryInfo = new DirectoryInfo(_output.Connection.Folder);
                if (directoryInfo.Exists) {
                    directoryInfo.Delete(true);
                }
            } catch (Exception ex) {
                response.Code = 500;
                response.Message = $"Could not delete {_output.Connection.Folder}. {ex.Message}";
            }
            return response;
        }
    }
}

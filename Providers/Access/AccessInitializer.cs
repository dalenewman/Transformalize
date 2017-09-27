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

namespace Transformalize.Providers.Access {
    public class AccessInitializer : IInitializer {
        private readonly OutputContext _output;
        private readonly IAction _adoInitializer;

        public AccessInitializer(IAction adoInitializer, OutputContext output) {
            _adoInitializer = adoInitializer;
            _output = output;
        }
        public ActionResponse Execute() {

            try {
                var fileInfo = new FileInfo(_output.Connection.File);
                if (fileInfo.DirectoryName != null) {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }
                if (fileInfo.Exists)
                    return _adoInitializer.Execute();

                var sourceFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files\\access", "empty.mdb"));
                File.Copy(sourceFile.FullName, fileInfo.FullName, true);

                return _adoInitializer.Execute();
            } catch (Exception e) {
                return new ActionResponse(500, e.Message) { Action = new Configuration.Action { Type = "internal", ErrorMode = "abort" } };
            }

        }
    }
}

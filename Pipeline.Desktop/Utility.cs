#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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

namespace Pipeline.Desktop {
    public static class Utility {
        static readonly char[] Slash = { '\\' };

        public static string GetTemporaryFolder(string processName) {
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd(Slash);

            //i.e. c: no user profile exists
            if (local.Length <= 2) {
                if (AppDomain.CurrentDomain.GetData("DataDirectory") != null) {
                    local = AppDomain.CurrentDomain.GetData("DataDirectory").ToString().TrimEnd(Slash);
                }
            }

            var folder = local + Constants.ApplicationFolder + processName;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }
    }
}

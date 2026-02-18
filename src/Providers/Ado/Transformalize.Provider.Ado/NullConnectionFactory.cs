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

using System.Data;
using Transformalize.Configuration;

namespace Transformalize.Providers.Ado {
   public class NullConnectionFactory : IConnectionFactory {
      public AdoProvider AdoProvider { get; set; } = AdoProvider.None;
      public string Terminator { get; set; } = string.Empty;

      public IDbConnection GetConnection(string appName = null) {
         return null;
      }

      public string GetConnectionString(string appName = null) {
         return string.Empty;
      }

      public string SqlDataType(Field field) {
         return string.Empty;
      }

      public string Enclose(string name) {
         return name;
      }

      public bool SupportsLimit { get; set; }

   }
}
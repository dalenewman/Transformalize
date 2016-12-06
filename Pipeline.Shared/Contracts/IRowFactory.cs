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

using System.Collections.Generic;

namespace Transformalize.Contracts {
    public interface IRowFactory {
        IRow Create();

        /// <summary>
        /// Create a new row and copy field values into it.
        /// </summary>
        /// <param name="row">The row to be cloned.</param>
        /// <param name="fields">You have to pass in the fields to clone because IRow doesn't know about the fields.</param>
        /// <returns>A copy of row.</returns>
        IRow Clone(IRow row, IEnumerable<IField> fields);
    }
}
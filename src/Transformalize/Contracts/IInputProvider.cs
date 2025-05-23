﻿#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Transformalize.Configuration;

namespace Transformalize.Contracts {
    public interface IInputProvider {
        /// <summary>
        /// Get the maximum input version respecting the filter, or null if no version is defined
        /// </summary>
        object GetMaxVersion();

        Schema GetSchema(Entity entity = null);

        /// <summary>
        /// Read all or just what is necessary from the input
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRow> Read();

    }
}

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
using System.Collections.Generic;

namespace Transformalize.Contracts {

    /// <inheritdoc />
    /// <summary>
    /// all transformers should implement this, they need to transform the data and Increment()
    /// </summary>
    public interface ITransform : IDisposable {

        IContext Context { get; }
        bool Run { get; set; }

        /// <summary>
        /// This transforms the row in the pipeline.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        IRow Transform(IRow row);

        IEnumerable<IRow> Transform(IEnumerable<IRow> rows);

        string Returns { get; set; }

        void Error(string error);
        void Warn(string warning);
        IEnumerable<string> Errors();
        IEnumerable<string> Warnings();
    }

}
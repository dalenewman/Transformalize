#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
namespace Pipeline.Contracts {

    /// <summary>
    /// all transformers should implement this, they need to transform the data and Increment()
    /// </summary>
    public interface ITransform {

        IContext Context { get; }

        /// <summary>
        /// This transforms the row in the pipeline.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        IRow Transform(IRow row);
    }

}
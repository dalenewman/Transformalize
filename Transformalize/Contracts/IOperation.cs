#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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

    public interface IOperation : IDisposable {

        /// <summary>
        /// Gives the operation access to it's place in the pipeline (Process, Entity, Field, Operation)
        /// </summary>
        IContext Context { get; }

        /// <summary>
        /// Whether or not this operation should run.  It can be set to false in operation's constructor.
        /// </summary>
        bool Run { get; set; }

        /// <summary>
        /// Allows the operation author to add errors to the pipeline's error log
        /// </summary>
        /// <param name="error"></param>
        void Error(string error);
        IEnumerable<string> Errors();

        /// <summary>
        /// Allows the operation author to add warnings to the pipeline's warning log
        /// </summary>
        /// <param name="warning"></param>
        void Warn(string warning);
        IEnumerable<string> Warnings();

        /// <summary>
        /// Author can code the operation, taking a row, returning a modified row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        IRow Operate(IRow row);

        /// <summary>
        /// Author can code around the row enumeration if desired, to add and remove rows, or whatever
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        IEnumerable<IRow> Operate(IEnumerable<IRow> rows);

        /// <summary>
        /// registers short-hand signature
        /// </summary>
        /// <returns></returns>
        IEnumerable<OperationSignature> GetSignatures();

    }
}
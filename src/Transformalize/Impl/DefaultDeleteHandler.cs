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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Impl {

   public class DefaultDeleteHandler : IEntityDeleteHandler {

      private readonly IContext _context;
      private readonly IRead _inputReader;
      private readonly IRead _outputReader;
      private readonly IDelete _outputDeleter;
      private readonly List<ITransform> _transforms = new List<ITransform>();

      public DefaultDeleteHandler(
          IContext context,
          IRead inputReader,
          IRead outputReader,
          IDelete outputDeleter
          ) {
         _context = context;
         _inputReader = inputReader;
         _outputReader = outputReader;
         _outputDeleter = outputDeleter;
      }

      public IEnumerable<IRow> DetermineDeletes() {
         var input = _inputReader.Read();
         if (_context.Entity.Pipeline == "parallel.linq") {
            input = input.AsParallel();
         }

         // I believe this is here in case the primary key depends on transformations
         var transformed = _transforms.Aggregate(input, (current, transform) => current.Select(transform.Operate));

         var output = _outputReader.Read();
         if (_context.Entity.Pipeline == "parallel.linq") {
            output = output.AsParallel();
         }

         return output.Except(transformed, new KeyComparer(_context.Entity.GetPrimaryKey()));
      }

      public void Delete() {
         if (_context.Process.Mode != "init") {
            _outputDeleter.Delete(DetermineDeletes());
         }
      }

      public void Register(ITransform transform) {
         _context.Debug(() => $"Registering {transform.GetType().Name}.");
         _transforms.Add(transform);
      }

      public void Register(IEnumerable<ITransform> transforms) {
         foreach (var transform in transforms) {
            Register(transform);
         }
      }
   }
}


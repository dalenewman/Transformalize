#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2022 Dale Newman
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
using System.IO;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.GeoJson {

   /// <summary>
   /// Writes GeoJson out to file
   /// </summary>
   public class GeoJsonFileWriter : IWrite {
      private readonly OutputContext _context;
      private IWrite _streamWriter;
      private readonly MemoryStream _stream;

      public bool UseAsyncMethods { get; set; }

      /// <summary>
      /// Given an output context, prepare to write a GeoJson file.
      /// </summary>
      /// <param name="context">a transformalize output context</param>
      public GeoJsonFileWriter(OutputContext context) {
         _context = context;
         _stream = new MemoryStream();
      }

      /// <summary>
      /// Given rows, write them to a GeoJson file.
      /// </summary>
      /// <param name="rows">transformalize rows</param>
      public void Write(IEnumerable<IRow> rows) {

         _streamWriter = UseAsyncMethods ? (IWrite) new GeoJsonStreamWriter(_context, _stream) : new GeoJsonStreamWriterSync(_context, _stream);

         using (var fileStream = File.Create(_context.Connection.File)) {
            _streamWriter.Write(rows);
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.CopyTo(fileStream);
         }
      }
   }
}
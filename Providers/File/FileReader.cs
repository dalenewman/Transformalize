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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.File {

   public class FileReader : IRead {

      private readonly InputContext _context;
      private readonly IRowFactory _rowFactory;
      private readonly Field _field;
      private readonly HashSet<int> _linesToKeep = new HashSet<int>();
      private readonly FileInfo _fileInfo;

      public FileReader(InputContext context, IRowFactory rowFactory) {
         _context = context;
         _rowFactory = rowFactory;
         _field = context.Entity.GetAllFields().First(f => f.Input);
         foreach (var transform in context.Entity.GetAllTransforms().Where(t => t.Method == "line")) {
            if (int.TryParse(transform.Value, out var lineNo)) {
               _linesToKeep.Add(lineNo);
            }
         }

         _fileInfo = FileUtility.Find(context.Connection.File);
      }

      public static IEnumerable<string> ReadLines(string path, Encoding encoding) {
         using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
         using (var sr = new StreamReader(fs, encoding)) {
            string line;
            while ((line = sr.ReadLine()) != null) {
               yield return line;
            }
         }
      }

      public IEnumerable<IRow> Read() {
         var encoding = Encoding.GetEncoding(_context.Connection.Encoding);
         var lineNo = 0;
         if (_fileInfo.Extension == ".xml") {
            var row = _rowFactory.Create();
            row[_field] = System.IO.File.ReadAllText(_fileInfo.FullName, encoding);
            yield return row;
         } else {
            if (_context.Connection.LinePattern != string.Empty) {

               var regex = new Regex(_context.Connection.LinePattern, RegexOptions.Compiled);
               var prevLine = string.Empty;

               foreach (var line in ReadLines(_fileInfo.FullName, encoding)) {
                  ++lineNo;

                  if (_linesToKeep.Contains(lineNo)) {
                     _context.Connection.Lines[lineNo] = line;
                  }

                  if (lineNo < _context.Connection.Start) continue;

                  if (regex.IsMatch(line)) { // CURRENT LINE PASSES

                     if (regex.IsMatch(prevLine)) {  // PREVIOUS LINE PASSES
                        var row = _rowFactory.Create();
                        row[_field] = string.Copy(prevLine);
                        prevLine = line;
                        yield return row;
                     } else { // PREVIOUS LINE FAILS
                        prevLine = line;
                     }

                  } else { // CURRENT LINE FAILS 
                     var combined = prevLine + " " + line;

                     if (regex.IsMatch(prevLine)) {

                        if (regex.IsMatch(combined)) { // IF COMBINED THEY STILL PASS, COMBINE AND CONTINUE
                           prevLine = combined;
                        } else { // IF COMBINED THEY FAIL, LET THE VALID PREVIOUS LINE THROUGH AND PUT LINE IN PREV LINE IN HOPES SUBSEQUENT LINES WILL MAKE IT PASS
                        var row = _rowFactory.Create();
                        row[_field] = string.Copy(prevLine);
                        prevLine = line;
                        yield return row;
                        }
                        
                     } else {
                        prevLine = combined;
                     }
                  }
               }

               if (regex.IsMatch(prevLine)) {
                  var row = _rowFactory.Create();
                  row[_field] = prevLine;
                  yield return row;
               }

            } else {
               foreach (var line in ReadLines(_fileInfo.FullName, encoding)) {

                  ++lineNo;

                  if (_linesToKeep.Contains(lineNo)) {
                     _context.Connection.Lines[lineNo] = line;
                  }

                  if (lineNo < _context.Connection.Start) continue;

                  var row = _rowFactory.Create();
                  row[_field] = line;
                  yield return row;

               }
            }
         }
      }
   }
}
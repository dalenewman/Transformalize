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
using System.Linq;
using SolrNet;
using SolrNet.Exceptions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Solr {

   public class SolrWriter2 : IWrite {

      private readonly OutputContext _context;
      readonly ISolrOperations<Dictionary<string, object>> _solr;
      private readonly Field[] _fields;
      private readonly List<Dictionary<string,object>> _documents;

      public SolrWriter2(OutputContext context, ISolrOperations<Dictionary<string, object>> solr) {
         _context = context;
         _solr = solr;
         _fields = context.OutputFields.Where(f => f.Type != "byte[]").ToArray();

         // initialize a batch of dictionaries to be re-used when adding documents to SOLR
         _documents = new List<Dictionary<string, object>>(_context.Entity.InsertSize);
         for (int i = 0; i < _context.Entity.InsertSize; i++) {
            var doc = new Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
            foreach (var field in _fields) {
               doc[field.Alias.ToLower()] = null;
            }
            _documents.Add(doc);
         }
      }

      public void Write(IEnumerable<IRow> rows) {
         var fullCount = 0;

         foreach (var part in rows.Partition(_context.Entity.InsertSize)) {
            var batchCount = 0;
            foreach (var row in part) {
               var doc = _documents[batchCount];
               foreach(var field in _fields) {
                  doc[field.Alias] = row[field];
               }
               batchCount++;
               fullCount++;
            }
            var response = _solr.AddRange(_documents.Take(batchCount));

            if (response.Status == 0) {
               var count = batchCount;
               _context.Debug(() => $"{count} to output");
            } else {
               _context.Error($"Couldn't add range of documents to SOLR.");
            }
         }

         if (fullCount > 0) {

            try {
               var commit = _solr.Commit();
               if (commit.Status == 0) {
                  _context.Entity.Inserts += System.Convert.ToUInt32(fullCount);
                  _context.Info($"Committed {fullCount} documents in {TimeSpan.FromMilliseconds(commit.QTime)}");
               } else {
                  _context.Error($"Failed to commit {fullCount} documents.  SOLR returned status {commit.Status}.");
               }
            } catch (SolrNetException ex) {
               _context.Error($"Failed to commit {fullCount} documents. {ex.Message}");
            }
         }
      }
   }
}

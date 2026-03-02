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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SolrNet;
using SolrNet.Exceptions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Solr {

   public class ParallelSolrWriter : IWrite {

      private readonly OutputContext _context;
      readonly ISolrOperations<Dictionary<string, object>> _solr;
      private readonly Field[] _fields;
      private int _count;
      private readonly ParallelOptions _options;
      private int _originalConnectionLimit;

      public ParallelSolrWriter(OutputContext context, ISolrOperations<Dictionary<string, object>> solr) {
         _context = context;
         _solr = solr;
         _fields = context.OutputFields.Where(f => f.Type != "byte[]").ToArray();
         _options = new ParallelOptions() { MaxDegreeOfParallelism = context.Connection.MaxDegreeOfParallelism };
      }

      public void Write(IEnumerable<IRow> rows) {

         _originalConnectionLimit = ServicePointManager.DefaultConnectionLimit;
         ServicePointManager.DefaultConnectionLimit = _context.Connection.MaxDegreeOfParallelism * 2;

         try {
            Parallel.ForEach(rows.Partition(_context.Entity.InsertSize), _options, part => {

               var docs = new List<Dictionary<string, object>>();
               foreach (var row in part) {
                  Interlocked.Increment(ref _count);
                  docs.Add(_fields.ToDictionary(field => field.Alias.ToLower(), field => row[field]));
               }

               var response = _solr.AddRange(docs);
               if (response.Status != 0) {
                  _context.Error($"Couldn't add range of {docs.Count} document{docs.Count.Plural()} to SOLR.");
               }
            });
         } catch (AggregateException ex) {
            foreach (var exception in ex.InnerExceptions) {
               _context.Error(exception.Message);
               _context.Error(exception.StackTrace);
            }
            return;
         }

         ServicePointManager.DefaultConnectionLimit = _originalConnectionLimit;

         if (_count > 0) {
            try {
               var commit = _solr.Commit();
               if (commit.Status == 0) {
                  _context.Entity.Inserts += Convert.ToUInt32(_count);
                  _context.Info($"Committed {_count} documents in {TimeSpan.FromMilliseconds(commit.QTime)}");
               } else {
                  _context.Error($"Failed to commit {_count} documents. SOLR returned status {commit.Status}.");
               }
            } catch (SolrNetException ex) {
               _context.Error($"Failed to commit {_count} documents. {ex.Message}");
            }
         }
      }
   }
}

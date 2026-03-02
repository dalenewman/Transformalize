#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2018 Dale Newman
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
#region license
#endregion

using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Solr {

   public class SolrInitializer : IInitializer {

      private readonly ISolrCoreAdmin _admin;
      private readonly ISolrOperations<Dictionary<string, object>> _solr;
      private readonly OutputContext _context;
      private readonly ITemplateEngine _schemaEngine;
      private readonly ITemplateEngine _configEngine;

      public SolrInitializer(
         OutputContext context, 
         ISolrCoreAdmin admin, 
         ISolrOperations<Dictionary<string, object>> solr, 
         ITemplateEngine schemaEngine,
         ITemplateEngine configEngine) {
         _context = context;
         _admin = admin;
         _solr = solr;
         _schemaEngine = schemaEngine;
         _configEngine = configEngine;
      }
      public ActionResponse Execute() {

         _context.Warn("Initializing");

         List<CoreResult> cores = null;
         try {
            cores = _admin.Status();
         } catch (SolrConnectionException ex) {
            return new ActionResponse { Code = 500, Message = $"Count not access {_context.Connection.Url}: {ex.Message}" };
         }

         var coreFolder = new DirectoryInfo(Path.Combine(_context.Connection.Folder, _context.Connection.Core));

         if (!coreFolder.Exists) {
            try {
               coreFolder.Create();
            } catch (UnauthorizedAccessException ex) {
               _context.Warn("Unable to create core folder: {0}", ex.Message);
            }
         }

         // https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
         var sourceFolder = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files\\solr"));

         try {
            foreach (var d in Directory.GetDirectories(sourceFolder.FullName, "*", SearchOption.AllDirectories)) {
               Directory.CreateDirectory(d.Replace(sourceFolder.FullName, coreFolder.FullName));
            }

            var fileCount = 0;
            _context.Debug(() => $"Copying SOLR files to {coreFolder.FullName}.");
            foreach (var f in Directory.GetFiles(sourceFolder.FullName, "*.*", SearchOption.AllDirectories)) {
               var solrFileInfo = new FileInfo(f.Replace(sourceFolder.FullName, coreFolder.FullName));

               File.Copy(f, solrFileInfo.FullName, true);
               _context.Debug(() => $"Copied {solrFileInfo.Name}.");
               fileCount++;
            }
            _context.Info($"Copied {fileCount} SOLR file{fileCount.Plural()}.");

            File.WriteAllText(Path.Combine(Path.Combine(coreFolder.FullName, "conf"), "schema.xml"), _schemaEngine.Render());
            File.WriteAllText(Path.Combine(Path.Combine(coreFolder.FullName, "conf"), "solrconfig.xml"), _configEngine.Render());

         } catch (UnauthorizedAccessException ex) {
            _context.Warn("Unable to transfer configuration files to core folder: {0}", ex.Message);
         }

         if (cores.Any(c => c.Name == _context.Connection.Core)) {

            try {
               _admin.Reload(_context.Connection.Core);
            } catch (SolrConnectionException ex) {
               _context.Warn("Unable to reload core");
               _context.Warn(ex.Message);
            }
            
         } else {

            try {
               _admin.Create(_context.Connection.Core, _context.Connection.Core);
            } catch (SolrConnectionException ex) {
               _context.Warn("Unable to create core");
               _context.Error(ex, ex.Message);
            }

         }

         _solr.Delete(SolrQuery.All);
         // _solr.Commit();  /* wait until after writing the new records */

         return new ActionResponse();
      }
   }
}
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
#region license
#endregion
using Transformalize.Actions;
using Transformalize.Contracts;
using SolrNet;
using Transformalize.Context;
using System.Net;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SolrNet.Impl;

namespace Transformalize.Provider.Solr {

    public class SolrInitializer : IInitializer {

        private readonly ISolrCoreAdmin _admin;
        private readonly ISolrOperations<Dictionary<string, object>> _solr;
        private readonly OutputContext _context;
        private readonly ITemplateEngine _engine;

        public SolrInitializer(OutputContext context, ISolrCoreAdmin admin, ISolrOperations<Dictionary<string, object>> solr, ITemplateEngine engine) {
            _context = context;
            _admin = admin;
            _solr = solr;
            _engine = engine;
        }
        public ActionResponse Execute() {

            var cores = new List<CoreResult>();
            try {
                cores = _admin.Status();
            } catch (Exception ex) {
                return new ActionResponse() { Code = 500, Message = $"Count not access {_context.Connection.Url}: {ex.Message}" };
            }

            var coreFolder = new DirectoryInfo(Path.Combine(_context.Connection.Folder, _context.Connection.Core));

            if (!coreFolder.Exists) {
                coreFolder.Create();
                // https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
                var sourceFolder = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Files\\solr"));
                foreach (string d in Directory.GetDirectories(sourceFolder.FullName, "*", SearchOption.AllDirectories)) {
                    Directory.CreateDirectory(d.Replace(sourceFolder.FullName, coreFolder.FullName));
                }
                foreach (string f in Directory.GetFiles(sourceFolder.FullName, "*.*", SearchOption.AllDirectories)) {
                    File.Copy(f, f.Replace(sourceFolder.FullName, coreFolder.FullName), true);
                }
            }

            File.WriteAllText(Path.Combine(Path.Combine(coreFolder.FullName, "conf"), "schema.xml"), _engine.Render());

            // todo: get status of cores, if core exists, can't "create" it, have to reload it, etc.
            if (cores.Any(c => c.Name == _context.Connection.Core)) {
                _admin.Reload(_context.Connection.Core);
            } else {
                _admin.Create(_context.Connection.Core, _context.Connection.Core);
            }

            _solr.Delete(SolrQuery.All);  // note: commit happens in writer

            return new ActionResponse();
        }
    }
}
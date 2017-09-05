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
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Transformalize.Contracts;
using Field = Transformalize.Configuration.Field;

namespace Transformalize.Providers.OpenXml {
    public class WordWriter : IWrite {

        private readonly FileInfo _fileInfo;
        private readonly Field[] _fields;
        private readonly IConnectionContext _connectionContext;

        public WordWriter(IConnectionContext context) {
            _connectionContext = context;
            _fileInfo = new FileInfo(context.Connection.File);
            _fields = context.Entity.GetAllOutputFields().Where(f => !f.System).ToArray();
        }

        public void Write(IEnumerable<IRow> rows) {
            WordprocessingDocument doc;

            var fileInfo = new FileInfo(_connectionContext.Connection.File);

            if (fileInfo.Extension.Equals(".doc", StringComparison.OrdinalIgnoreCase)) {
                doc = WordprocessingDocument.Open(new DocConverter().Convert(_connectionContext), true);
            } else {
                doc = WordprocessingDocument.Open(fileInfo.FullName, true);
            }

            var mailMerges = doc.MainDocumentPart.DocumentSettingsPart.GetXDocument().Descendants().Where(d => d.Name == W.mailMerge);

            foreach (var mm in mailMerges) {
                Console.WriteLine(mm.Value);
            }

            foreach (var row in rows) {
            }
        }

    }
}

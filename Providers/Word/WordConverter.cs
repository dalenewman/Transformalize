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
using System.IO;
using DIaLOGIKa.b2xtranslator.DocFileFormat;
using DIaLOGIKa.b2xtranslator.OpenXmlLib.WordprocessingML;
using DIaLOGIKa.b2xtranslator.StructuredStorage.Reader;
using DIaLOGIKa.b2xtranslator.WordprocessingMLMapping;
using Transformalize.Contracts;

namespace Word {
    public class WordConverter : IConvertFile {

        /// <summary>
        /// Converts from .doc to .docx and returns output (.docx) file name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Convert(IConnectionContext context) {

            var outFile = Path.GetTempPath() + Guid.NewGuid() + ".docx";
            var fileInfo = new FileInfo(context.Connection.File);

            context.Info("Converting doc to docx");

            using (var reader = new StructuredStorageReader(fileInfo.FullName)) {
                var doc = new WordDocument(reader);
                var outType = Converter.DetectOutputType(doc);
                var conformOutputFile = Converter.GetConformFilename(outFile, outType);
                var docx = WordprocessingDocument.Create(conformOutputFile, outType);
                Converter.Convert(doc, docx);
            }

            return outFile;

        }
    }
}

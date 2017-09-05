using System;
using System.IO;
using DIaLOGIKa.b2xtranslator.DocFileFormat;
using DIaLOGIKa.b2xtranslator.OpenXmlLib.WordprocessingML;
using DIaLOGIKa.b2xtranslator.StructuredStorage.Reader;
using DIaLOGIKa.b2xtranslator.WordprocessingMLMapping;
using Transformalize.Contracts;

namespace Transformalize.Providers.OpenXml {
    public class DocConverter {

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

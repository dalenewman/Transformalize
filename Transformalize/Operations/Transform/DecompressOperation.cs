using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {

    public class DecompressOperation : ShouldRunOperation {
        public DecompressOperation(string inKey, string outKey) : base(inKey, outKey) { }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = Decompress(row[InKey].ToString());
                    yield return row;
                } else {
                    Interlocked.Increment(ref SkipCount);
                    yield return row;
                }
            }
        }

        protected static string Decompress(string compressedText) {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream()) {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress)) {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }


    }

}
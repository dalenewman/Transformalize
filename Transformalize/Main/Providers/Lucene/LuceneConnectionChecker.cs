using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Lucene {

    public class LuceneConnectionChecker : IConnectionChecker {
        private readonly string _processName;
        private static readonly Dictionary<int, bool> Checks = new Dictionary<int, bool>();

        public LuceneConnectionChecker(string processName) {
            _processName = processName;
        }

        public bool Check(AbstractConnection connection) {
            var hashCode = connection.Folder.GetHashCode();
            if (Checks.ContainsKey(hashCode)) {
                return Checks[hashCode];
            }

            try {
                using (var indexDirectory = LuceneDirectoryFactory.Create(connection, connection.TflBatchEntity(_processName))) {
                    using (var reader = IndexReader.Open(indexDirectory, true)) {
                        TflLogger.Debug(_processName, string.Empty, "Successfully connected to lucene index in {0}.", connection.Folder);
                        Checks[hashCode] = true;
                        return true;
                    }
                }
            } catch (Exception ex) {
                TflLogger.Warn(_processName, string.Empty, "Failed to connect to a lucene index in {0}.", connection.Folder);
                TflLogger.Debug(_processName, string.Empty, ex.Message);
                var exists = new DirectoryInfo(connection.Folder).Exists;
                Checks[hashCode] = exists;
                return exists;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Index;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Lucene {

    public class LuceneConnectionChecker : IConnectionChecker {
        private readonly string _processName;
        private readonly ILogger _logger;
        private static readonly Dictionary<int, bool> Checks = new Dictionary<int, bool>();

        public LuceneConnectionChecker(string processName, ILogger logger) {
            _processName = processName;
            _logger = logger;
        }

        public bool Check(AbstractConnection connection) {
            var hashCode = connection.Folder.GetHashCode();
            if (Checks.ContainsKey(hashCode)) {
                return Checks[hashCode];
            }

            try {
                using (var indexDirectory = LuceneDirectoryFactory.Create(connection, connection.TflBatchEntity(_processName))) {
                    using (var reader = IndexReader.Open(indexDirectory, true)) {
                        connection.Logger.Debug("Successfully connected to lucene index in {0}.", connection.Folder);
                        Checks[hashCode] = true;
                        return true;
                    }
                }
            } catch (Exception ex) {
                _logger.Warn("Failed to connect to a lucene index in {0}.", connection.Folder);
                _logger.Debug(ex.Message);
                var exists = new DirectoryInfo(connection.Folder).Exists;
                Checks[hashCode] = exists;
                return exists;
            }
        }
    }
}
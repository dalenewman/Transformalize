using System;
using System.Collections.Generic;
using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneConnectionChecker : IConnectionChecker {
        private readonly Logger _log = LogManager.GetLogger("tfl");
        private static readonly Dictionary<int, bool> Checks = new Dictionary<int, bool>();

        public bool Check(AbstractConnection connection) {
            var hashCode = connection.Folder.GetHashCode();
            if (Checks.ContainsKey(hashCode)) {
                return Checks[hashCode];
            }

            try {
                using (var indexDirectory = LuceneDirectoryFactory.Create(connection)) {
                    using (var reader = IndexReader.Open(indexDirectory, true)) {
                        _log.Debug("Successfully connected to lucene index in {0}.", connection.Folder);
                        return true;
                    }
                }
            } catch (Exception ex) {
                _log.Warn("Failed to connect to lucene index in {0}. {1}", connection.Folder, ex.Message);
                return false;
            }
        }
    }
}
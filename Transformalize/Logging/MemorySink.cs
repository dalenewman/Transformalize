using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Libs.SemanticLogging.Formatters;

namespace Transformalize.Logging {

    public sealed class MemorySink : IObserver<EventEntry> {
        private readonly List<string> _log;

        private readonly IEventTextFormatter _formatter = new LegacyLogFormatter();

        public MemorySink(ref List<string> log) {
            _log = log;
        }

        public void OnNext(EventEntry entry) {
            if (entry == null)
                return;
            using (var writer = new StringWriter()) {
                _formatter.WriteEvent(entry, writer);
                _log.Add(writer.ToString());
            }
        }

        public void OnCompleted() {
        }

        public void OnError(Exception error) {
        }
    }

}

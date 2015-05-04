using System.IO;
using Transformalize.Run.Libs.SemanticLogging;
using Transformalize.Run.Libs.SemanticLogging.Formatters;

namespace Transformalize.Run.Logging
{
    public class LegacyLogFormatter : IEventTextFormatter {
        public void WriteEvent(EventEntry eventEntry, TextWriter writer) {
            writer.Write(eventEntry.GetFormattedTimestamp("HH:mm:ss | "));
            writer.WriteLine(eventEntry.FormattedMessage);
        }
    }
}
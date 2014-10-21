using System.IO;
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging;
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Formatters;

namespace Transformalize.Logging
{
    public class LegacyLogFormatter : IEventTextFormatter {
        public void WriteEvent(EventEntry eventEntry, TextWriter writer) {
            writer.Write(eventEntry.GetFormattedTimestamp("HH:mm:ss | "));
            writer.WriteLine(eventEntry.FormattedMessage);
        }
    }
}
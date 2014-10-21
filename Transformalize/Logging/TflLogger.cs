using System.Diagnostics.Tracing;
using System.Text;
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging;
using Transformalize.Operations.Transform;

namespace Transformalize.Logging {

    public static class TflLogger {

        private const string DELIMITER = " | ";
        private const int PROCESS_LENGTH = 12;
        private const int ENTITY_LENGTH = 18;
        private static readonly ObjectPool<StringBuilder> StringBuilders = new ObjectPool<StringBuilder>(() => new StringBuilder());
        public static bool IsDebugEnabled { get { return TflEventSource.Log.IsEnabled(EventLevel.LogAlways, Keywords.All); } }
        public static bool IsErrorEnabled { get { return TflEventSource.Log.IsEnabled(EventLevel.Error, Keywords.All); } }
        public static bool IsWarnEnabled { get { return TflEventSource.Log.IsEnabled(EventLevel.Warning, Keywords.All); } }
        public static bool IsInfoEnabled { get { return TflEventSource.Log.IsEnabled(EventLevel.Informational, Keywords.All); } }

        private static StringBuilder GetStringBuilder(string level, string process, string entity) {
            var sb = StringBuilders.GetObject();
            sb.Append(level);
            sb.Append(DELIMITER);
            LengthAppend(process, ref sb, PROCESS_LENGTH);
            sb.Append(DELIMITER);
            LengthAppend(entity, ref sb, ENTITY_LENGTH);
            sb.Append(DELIMITER);
            return sb;
        }

        private static void LengthAppend(string input, ref StringBuilder sb, int limit) {
            if (input == null) {
                sb.Append('.', limit);
                return;
            }
            var length = input.Length;
            if (length >= limit) {
                sb.Append(input, 0, limit);
            } else {
                sb.Append(input);
                sb.Append('.', limit - length);
            }
        }

        public static void Info(string process, string entity, string message, params object[] args) {
            if (!IsInfoEnabled)
                return;
            var sb = GetStringBuilder("Info ", process, entity);
            if (args != null && args.Length > 0) {
                sb.AppendFormat(message, args);
            } else {
                sb.Append(message);
            }
            TflEventSource.Log.Info(sb.ToString(), process, entity);
            sb.Clear();
            StringBuilders.PutObject(sb);
        }

        public static void Debug(string process, string entity, string message, params object[] args) {
            if (!IsDebugEnabled)
                return;

            var sb = GetStringBuilder("Debug", process, entity);
            if (args != null && args.Length > 0) {
                sb.AppendFormat(message, args);
            } else {
                sb.Append(message);
            }
            TflEventSource.Log.Debug(sb.ToString(), process, entity);
            sb.Clear();
            StringBuilders.PutObject(sb);
        }

        public static void Warn(string process, string entity, string message, params object[] args) {
            if (!IsWarnEnabled)
                return;

            var sb = GetStringBuilder("Warn ", process, entity);
            if (args != null && args.Length > 0) {
                sb.AppendFormat(message, args);
            } else {
                sb.Append(message);
            }
            TflEventSource.Log.Warn(sb.ToString(), process, entity);

            sb.Clear();
            StringBuilders.PutObject(sb);
        }

        public static void Error(string process, string entity, string message, params object[] args) {
            if(!IsErrorEnabled)
                return;
            var sb = GetStringBuilder("Error", process, entity);
            if (args != null && args.Length > 0) {
                sb.AppendFormat(message, args);
            } else {
                sb.Append(message);
            }
            TflEventSource.Log.Error(sb.ToString(), process, entity);
            sb.Clear();
            StringBuilders.PutObject(sb);
        }

    }
}
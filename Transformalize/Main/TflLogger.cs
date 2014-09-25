using System.Data.Common;
using System.Text;
using Transformalize.Libs.NLog;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {

    public static class TflLogger {

        private static readonly Logger Log = LogManager.GetLogger("tfl");
        private const string DELIMITER = " | ";
        private const int PROCESS_LENGTH = 12;
        private const int ENTITY_LENGTH = 18;
        private static readonly ObjectPool<StringBuilder> StringBuilders = new ObjectPool<StringBuilder>(() => new StringBuilder());
        public static bool IsDebugEnabled { get { return Log.IsDebugEnabled; } }
        public static bool IsErrorEnabled { get { return Log.IsErrorEnabled; } }
        public static bool IsWarnEnabled { get { return Log.IsWarnEnabled; } }
        public static bool IsTraceEnabled { get { return Log.IsTraceEnabled; } }
        public static bool IsInfoEnabled { get { return Log.IsInfoEnabled; } }

        private static StringBuilder GetStringBuilder(string process, string entity) {
            var sb = StringBuilders.GetObject();
            LengthAppend(process, ref sb, PROCESS_LENGTH);
            sb.Append(DELIMITER);
            LengthAppend(entity, ref sb, ENTITY_LENGTH);
            sb.Append(DELIMITER);
            return sb;
        }

        private static void LengthAppend(string input, ref StringBuilder sb, int limit) {
            var length = input.Length;
            if (length >= limit) {
                sb.Append(input, 0, limit);
            } else {
                sb.Append(input);
                sb.Append('.', limit - length);
            }
        }

        public static void Info(string process, string entity, string message, params object[] args) {
            if (!Log.IsInfoEnabled)
                return;
            var sb = GetStringBuilder(process, entity);
            sb.AppendFormat(message, args);
            Log.Info(sb.ToString);
            sb.Clear();
            StringBuilders.PutObject(sb);
        }

        public static void Debug(string process, string entity, string message, params object[] args) {
            if (!Log.IsDebugEnabled)
                return;
            var sb = GetStringBuilder(process, entity);
            sb.AppendFormat(message, args);
            Log.Debug(sb.ToString);
            sb.Clear();
            StringBuilders.PutObject(sb);
        }

        public static void Warn(string process, string entity, string message, params object[] args) {
            if (!Log.IsWarnEnabled)
                return;
            var sb = GetStringBuilder(process, entity);
            sb.AppendFormat(message, args);
            Log.Warn(sb.ToString);
            sb.Clear();
            StringBuilders.PutObject(sb);
        }

        public static void Error(string process, string entity, string message, params object[] args) {
            if (!Log.IsErrorEnabled)
                return;
            var sb = GetStringBuilder(process, entity);
            sb.AppendFormat(message, args);
            Log.Error(sb.ToString);
            sb.Clear();
            StringBuilders.PutObject(sb);
        }

        public static void Trace(string process, string entity, string message, params object[] args) {
            if (!Log.IsTraceEnabled)
                return;
            var sb = GetStringBuilder(process, entity);
            sb.AppendFormat(message, args);
            Log.Trace(sb.ToString);
            sb.Clear();
            StringBuilders.PutObject(sb);
        }

    }
}
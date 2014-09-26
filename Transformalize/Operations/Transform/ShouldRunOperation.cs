using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public abstract class ShouldRunOperation : AbstractOperation {

        public Func<Row, bool> ShouldRun = row => true;
        public bool IsFilter = false;
        protected string InKey;
        protected string OutKey;
        protected int SkipCount = 0;
        protected ObjectPool<StringBuilder> StringBuilders = new ObjectPool<StringBuilder>(() => new StringBuilder());
        protected ObjectPool<StringWriter> StringWriters = new ObjectPool<StringWriter>(() => new StringWriter());

        protected ShouldRunOperation(string inKey, string outKey) {
            base.OnFinishedProcessing += TflOperation_OnFinishedProcessing;
            InKey = inKey;
            OutKey = outKey;
        }

        void TflOperation_OnFinishedProcessing(IOperation obj) {
            if (SkipCount > 0) {
                if (IsFilter) {
                    TflLogger.Info(ProcessName, EntityName, "Blocked {0} row{1}. Allowed {2} row{3}.", SkipCount, SkipCount.Plural(), obj.Statistics.OutputtedRows, obj.Statistics.OutputtedRows.Plural());
                } else {
                    TflLogger.Info(ProcessName, EntityName, "Skipped {0} of {1} row{2}.", SkipCount, obj.Statistics.OutputtedRows, obj.Statistics.OutputtedRows.Plural());
                }
            }
            var seconds = Convert.ToInt64(obj.Statistics.Duration.TotalSeconds);
            TflLogger.Info(ProcessName, EntityName, "Completed {0} rows in {1}: {2} second{3}.", obj.Statistics.OutputtedRows, Name, seconds, seconds.Plural());
        }

        protected static bool CanChangeType(object value, string simpleType) {
            var conversionType = Common.ToSystemType(simpleType);
            return CanChangeType(value, conversionType);
        }

        protected static bool CanChangeType(object value, Type conversionType) {
            if (conversionType == null) {
                return false;
            }

            if (value == null) {
                return false;
            }

            var convertible = value as IConvertible;

            return convertible != null;
        }

        protected void Skip() {
            Interlocked.Increment(ref SkipCount);
        }

    }
}
using Microsoft.AnalysisServices;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Xml;
using Transformalize.Configuration;
using Transformalize.Context;

namespace Transformalize.Provider.SSAS {
    public static class SSAS {

        public static IEnumerable<Field> GetFields(OutputContext output) {
            return output.Entity.GetAllOutputFields().Where(f => f.Type != "byte[]" && f.Type != "guid" && f.Type != "object" && !f.System);
        }

        public static string CreateQuery(OutputContext output) {
            var names = string.Join(",", GetFields(output).Select(f => f.Name == f.Alias ? $"[{f.Name}]" : $"[{f.Name}] AS [{f.Alias}]"));
            var sql = $"SELECT {names} FROM [{(output.Entity.Schema == string.Empty ? "dbo" : output.Entity.Schema)}].[{output.Entity.Name}] WITH (NOLOCK)";
            return sql;
        }

        public static bool Process(ProcessableMajorObject obj, ProcessType type, OutputContext output) {
            var errors = new ErrorConfiguration {
                KeyDuplicate = ErrorOption.ReportAndStop,
                KeyNotFound = ErrorOption.ReportAndStop,
                NullKeyNotAllowed = ErrorOption.ReportAndStop,
                CalculationError = ErrorOption.ReportAndStop,
                NullKeyConvertedToUnknown = ErrorOption.ReportAndStop
            };
            var warnings = new XmlaWarningCollection();
            obj.Process(type, errors, warnings);

            if (warnings.Count > 0) {
                foreach (XmlaWarning warning in warnings) {
                    output.Warn(warning.Description);
                }
                return false;
            }
            return true;
        }

        public static bool Save(Microsoft.AnalysisServices.Server server, OutputContext output, IMajorObject obj) {

            var builder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true })) {
                Scripter.WriteAlter(xmlWriter, obj, true, true);
                xmlWriter.Flush();
            }

            var command = builder.ToString();
            output.Debug(() => command);
            var results = server.Execute(command);

            if (results.Count > 0) {
                foreach (XmlaResult result in results) {
                    if (result.Messages.Count > 0) {
                        foreach (XmlaMessage message in result.Messages) {
                            output.Error(message.Description);
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool CanDistinctCount(OleDbType type) {
            switch (type) {
                case OleDbType.BigInt:
                case OleDbType.Currency:
                case OleDbType.Double:
                case OleDbType.Integer:
                case OleDbType.Single:
                case OleDbType.SmallInt:
                case OleDbType.TinyInt:
                case OleDbType.UnsignedBigInt:
                case OleDbType.UnsignedInt:
                case OleDbType.UnsignedSmallInt:
                case OleDbType.UnsignedTinyInt:
                    return true;
                default:
                    return false;
            }
        }

        public static OleDbType GetOleDbType(Field field) {
            var sysType = Constants.TypeSystem()[field.Type];
            if (ReferenceEquals(sysType, typeof(string))) {
                return field.Unicode ? OleDbType.WChar : OleDbType.Char;
            }
            if (ReferenceEquals(sysType, typeof(int))) {
                return OleDbType.Integer;
            }
            if (ReferenceEquals(sysType, typeof(long))) {
                return OleDbType.BigInt;
            }
            if (ReferenceEquals(sysType, typeof(short))) {
                return OleDbType.SmallInt;
            }
            if (ReferenceEquals(sysType, typeof(byte))) {
                return OleDbType.TinyInt;
            }
            if (ReferenceEquals(sysType, typeof(bool))) {
                return OleDbType.Boolean;
            }
            if (ReferenceEquals(sysType, typeof(System.DateTime))) {
                return OleDbType.Date;
            }
            if (ReferenceEquals(sysType, typeof(char))) {
                return OleDbType.Char;
            }
            if (ReferenceEquals(sysType, typeof(decimal))) {
                return field.Scale <= 4 ? OleDbType.Currency : OleDbType.Double;  // SSAS doesn't support OleDbType.Decimal
            }
            if (ReferenceEquals(sysType, typeof(double))) {
                return OleDbType.Double;
            }
            if (ReferenceEquals(sysType, typeof(float))) {
                return OleDbType.Single;
            }
            if (ReferenceEquals(sysType, typeof(byte[]))) {
                return OleDbType.Binary;
            }
            if (ReferenceEquals(sysType, typeof(System.Guid))) {
                return OleDbType.Guid;
            }
            if (ReferenceEquals(sysType, typeof(uint))) {
                return OleDbType.UnsignedInt;
            }
            if (ReferenceEquals(sysType, typeof(ulong))) {
                return OleDbType.UnsignedBigInt;
            }
            if (ReferenceEquals(sysType, typeof(ushort))) {
                return OleDbType.UnsignedSmallInt;
            }
            return OleDbType.WChar;
        }

    }
}

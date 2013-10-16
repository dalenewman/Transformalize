using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.fastJSON;
using Transformalize.Main;

namespace Transformalize.Operations.Transform
{
    public class FromJsonOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly bool _clean;
        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;
        private readonly JSON _json = JSON.Instance;
        private readonly Regex _start = new Regex(@"^ ?\{{1} ?""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _end = new Regex(@"""{1} ?\}{1} ?$", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _colon = new Regex(@"""{1}[: ]+""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _colonNum = new Regex(@"""{1}[: ]+(?=\d{1})", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _numColon = new Regex(@"(?<=\d)[, ]+""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _comma = new Regex(@"""{1}[, ]+""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _fix = new Regex(@"\\?""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _startBack = new Regex(@"^_SS_", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _endBack = new Regex(@"_EE_$", RegexOptions.Compiled | RegexOptions.Singleline);

        public FromJsonOperation(string inKey, bool clean, IParameters parameters) {
            _inKey = inKey;
            _clean = clean;
            _parameters = parameters.ToEnumerable();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var input = _clean ? Clean(row[_inKey]) : row[_inKey].ToString();
                var dict = _json.ToObject<Dictionary<string, object>>(input);

                foreach (var pair in _parameters) {
                    row[pair.Key] = Common.ObjectConversionMap[pair.Value.SimpleType](dict[pair.Value.Name]);
                }

                yield return row;
            }
        }

        /// <summary>
        /// An attempt to fix unescaped double quotes within the property or value in a single line of JSON
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string Clean(object input) {
            var output = _start.Replace(input.ToString(), "_SS_");  //tag start with valid quote
            output = _end.Replace(output, "_EE_"); // tag end with valid quote
            output = _colon.Replace(output, "_::_"); // tag colon with valid quotes
            output = _colonNum.Replace(output, "||||"); // tag colon with left side quote, but not right because value is numeric
            output = _numColon.Replace(output, "%%%%"); // tag colon with right side quote, but not left because value is numeric
            output = _comma.Replace(output, "_,,_"); // tag commas with valid quotes
            output = _fix.Replace(output, string.Empty); // just get rid of them
            output = _startBack.Replace(output, @"{"""); // put start back
            output = _endBack.Replace(output, @"""}"); // put end back
            output = output.Replace("_::_", @""":"""); // put valid colons back
            output = output.Replace("||||", @""":"); // put valid colons back
            output = output.Replace("%%%%", @":"""); // put valid colons back
            return output.Replace("_,,_", @""","""); // put valid commas back
        }
    }
}
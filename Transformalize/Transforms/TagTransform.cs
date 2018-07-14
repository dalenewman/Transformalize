#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class TagTransform : BaseTransform {

        internal class TagAttribute {

            public TagAttribute(IEnumerable<Field> input, string name, string value) {
                Name = name;
                Value = value;
                IsSet = value != string.Empty;
                Field = input.FirstOrDefault(f => f.Alias == value);
                IsField = Field != null;
            }
            public bool IsSet { get; }
            public bool IsField { get; }
            public Field Field { get; }
            public string Name { get; }
            public string Value { get; }

            public void Append(StringBuilder sb, IRow row, Func<string, string> encode) {
                if (!IsSet) return;
                sb.Append(' ');
                sb.Append(Name);
                sb.Append("=\"");
                sb.Append(IsField ? encode(row[Field].ToString()) : encode(Value));
                sb.Append("\"");
            }

            public int Length() {
                var length = Name.Length + 3;  // attribute name, =, 2 double quotes
                length += IsField ? (Field.Length == "max" ? 1024 : Convert.ToInt32(Field.Length)) : Value.Length;
                return length;
            }
        }

        private readonly List<TagAttribute> _attributes = new List<TagAttribute>();
        private readonly Func<IRow, object> _encode;
        private readonly bool _selfClosing = false;
        private readonly StringBuilder _sbOperate = new StringBuilder();
        private readonly StringBuilder _sbEncode = new StringBuilder();

        public TagTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (string.IsNullOrEmpty(Context.Operation.Tag)) {
                Error("The tag transform requires a tag (e.g. a, span, div, etc).");
                Run = false;
                return;
            }

            _selfClosing = Context.Operation.Tag.Equals("img", StringComparison.OrdinalIgnoreCase);

            if (Context.Operation.Class == string.Empty && Context.Field.Class != string.Empty) {
                Context.Operation.Class = Context.Field.Class;
            }

            if (Context.Operation.Style == string.Empty && Context.Field.Style != string.Empty) {
                Context.Operation.Style = Context.Field.Style;
            }

            if (Context.Operation.Role == string.Empty && Context.Field.Role != string.Empty) {
                Context.Operation.Role = Context.Field.Role;
            }

            if (Context.Operation.HRef == string.Empty && Context.Field.HRef != string.Empty) {
                Context.Operation.HRef = Context.Field.HRef;
            }

            if (Context.Operation.Target == string.Empty && Context.Field.Target != string.Empty) {
                Context.Operation.Target = Context.Field.Target;
            }

            if (Context.Operation.Body == string.Empty && Context.Field.Body != string.Empty) {
                Context.Operation.Body = Context.Field.Body;
            }

            if (Context.Operation.Src == string.Empty && Context.Field.Src != string.Empty) {
                Context.Operation.Src = Context.Field.Src;
            }

            if (Context.Operation.Width == 0 && Context.Field.Width > 0) {
                Context.Operation.Width = Context.Field.Width;
            }

            if (Context.Operation.Height == 0 && Context.Field.Height > 0) {
                Context.Operation.Height = Context.Field.Height;
            }

            var input = MultipleInput();
            _attributes.Add(new TagAttribute(input, "href", Context.Operation.HRef));
            _attributes.Add(new TagAttribute(input, "class", Context.Operation.Class));
            _attributes.Add(new TagAttribute(input, "title", Context.Operation.Title));
            _attributes.Add(new TagAttribute(input, "style", Context.Operation.Style));
            _attributes.Add(new TagAttribute(input, "role", Context.Operation.Role));
            _attributes.Add(new TagAttribute(input, "target", Context.Operation.Target));
            _attributes.Add(new TagAttribute(input, "src", Context.Operation.Src));
            _attributes.Add(new TagAttribute(input, "width", Context.Operation.Width == 0 ? string.Empty : Context.Operation.Width.ToString()));
            _attributes.Add(new TagAttribute(input, "height", Context.Operation.Height == 0 ? string.Empty : Context.Operation.Height.ToString()));

            var body = Context.Operation.Body == string.Empty ? input.First() : (input.FirstOrDefault(f => f.Alias == Context.Operation.Body) ?? input.FirstOrDefault(f => f.Name == Context.Operation.Body)) ?? input.First();

            if (!Context.Field.Raw) {
                Context.Field.Raw = true;
            }

            _encode = (row) => Context.Operation.Encode ? Encode(row[body].ToString()) : row[body];
        }

        public override IRow Operate(IRow row) {
            _sbOperate.Clear();

            // open

            _sbOperate.AppendFormat("<{0}", Context.Operation.Tag);

            // attributes
            foreach (var attribute in _attributes) {
                attribute.Append(_sbOperate, row, Encode);
            }

            if (_selfClosing) {
                _sbOperate.Append("/>");
            } else {
                _sbOperate.Append(">");

                // content
                _sbOperate.Append(_encode(row));

                // close
                _sbOperate.AppendFormat("</{0}>", Context.Operation.Tag);
            }

            row[Context.Field] = _sbOperate.ToString();

            return row;
        }

        private string Encode(string value) {
            _sbEncode.Clear();
            for (var i = 0; i < value.Length; i++) {
                var ch = value[i];
                if (ch <= '>') {
                    switch (ch) {
                        case '<':
                            _sbEncode.Append("&lt;");
                            break;
                        case '>':
                            _sbEncode.Append("&gt;");
                            break;
                        case '"':
                            _sbEncode.Append("&quot;");
                            break;
                        case '\'':
                            _sbEncode.Append("&#39;");
                            break;
                        case '&':
                            _sbEncode.Append("&amp;");
                            break;
                        default:
                            _sbEncode.Append(ch);
                            break;
                    }
                } else {
                    _sbEncode.Append(ch);
                }
            }
            return _sbEncode.ToString();
        }

        public override IEnumerable<OperationSignature> GetSignatures() {

            yield return new OperationSignature("tag") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("tag"),
                    new OperationParameter("class",""),
                    new OperationParameter("style",""),
                    new OperationParameter("title",""),
                    new OperationParameter("href",""),
                    new OperationParameter("role",""),
                    new OperationParameter("target",""),
                    new OperationParameter("body",""),
                    new OperationParameter("encode","true"),
                    new OperationParameter("src",""),
                    new OperationParameter("width","0"),
                    new OperationParameter("height","0")
                }
            };

        }
    }
}
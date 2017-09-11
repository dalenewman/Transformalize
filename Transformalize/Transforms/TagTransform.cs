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

            public void Append(StringBuilder sb, IRow row) {
                if (!IsSet) return;
                sb.Append(' ');
                sb.Append(Name);
                sb.Append("=\"");
                sb.Append(IsField ? Encode(row[Field].ToString()) : Encode(Value));
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

        public TagTransform(IContext context) : base(context, "string") {
            if (string.IsNullOrEmpty(context.Transform.Tag)) {
                Error("The tag transform requires a tag (e.g. a, span, div, etc).");
                Run = false;
                return;
            }

            _selfClosing = Context.Transform.Tag.Equals("img", StringComparison.OrdinalIgnoreCase);

            if (Context.Transform.Class == string.Empty && Context.Field.Class != string.Empty) {
                Context.Transform.Class = Context.Field.Class;
            }

            if (Context.Transform.Style == string.Empty && Context.Field.Style != string.Empty) {
                Context.Transform.Style = Context.Field.Style;
            }

            if (Context.Transform.Role == string.Empty && Context.Field.Role != string.Empty) {
                Context.Transform.Role = Context.Field.Role;
            }

            if (Context.Transform.HRef == string.Empty && Context.Field.HRef != string.Empty) {
                Context.Transform.HRef = Context.Field.HRef;
            }

            if (Context.Transform.Target == string.Empty && Context.Field.Target != string.Empty) {
                Context.Transform.Target = Context.Field.Target;
            }

            if (Context.Transform.Body == string.Empty && Context.Field.Body != string.Empty) {
                Context.Transform.Body = Context.Field.Body;
            }

            if (Context.Transform.Src == string.Empty && Context.Field.Src != string.Empty) {
                Context.Transform.Src = Context.Field.Src;
            }

            if (Context.Transform.Width == 0 && Context.Field.Width > 0) {
                Context.Transform.Width = Context.Field.Width;
            }

            if (Context.Transform.Height == 0 && Context.Field.Height > 0) {
                Context.Transform.Height = Context.Field.Height;
            }

            var input = MultipleInput();
            _attributes.Add(new TagAttribute(input, "href", Context.Transform.HRef));
            _attributes.Add(new TagAttribute(input, "class", Context.Transform.Class));
            _attributes.Add(new TagAttribute(input, "title", Context.Transform.Title));
            _attributes.Add(new TagAttribute(input, "style", Context.Transform.Style));
            _attributes.Add(new TagAttribute(input, "role", Context.Transform.Role));
            _attributes.Add(new TagAttribute(input, "target", Context.Transform.Target));
            _attributes.Add(new TagAttribute(input, "src", Context.Transform.Src));
            _attributes.Add(new TagAttribute(input, "width", Context.Transform.Width == 0 ? string.Empty : Context.Transform.Width.ToString()));
            _attributes.Add(new TagAttribute(input, "height", Context.Transform.Height == 0 ? string.Empty : Context.Transform.Height.ToString()));

            var body = Context.Transform.Body == string.Empty ? input.First() : (input.FirstOrDefault(f => f.Alias == Context.Transform.Body) ?? input.FirstOrDefault(f => f.Name == Context.Transform.Body)) ?? input.First();

            if (!Context.Field.Raw) {
                Context.Field.Raw = true;
            }

            if (Context.Field.Length != "max") {
                var calculatedLength = _attributes.Sum(a => a.Length()) + Context.Transform.Tag.Length + 5;  // 5 = <></>
                if (calculatedLength > Convert.ToInt32(Context.Field.Length)) {
                    Context.Warn($"The calculated length of {Context.Field.Alias} is {calculatedLength}, but it's length is set to {Context.Field.Length}.  Truncation may occur.  You need to set the length so it can accomadate tag characters, tag name, attributes, and the field's content.");
                }
            }

            _encode = (row) => Context.Transform.Encode ? Encode(row[body].ToString()) : row[body];
        }

        public override IRow Transform(IRow row) {
            var sb = new StringBuilder();

            // open

            sb.AppendFormat("<{0}", Context.Transform.Tag);

            // attributes
            foreach (var attribute in _attributes) {
                attribute.Append(sb, row);
            }

            if (_selfClosing) {
                sb.Append("/>");
            } else {
                sb.Append(">");

                // content
                sb.Append(_encode(row));

                // close
                sb.AppendFormat("</{0}>", Context.Transform.Tag);
            }

            row[Context.Field] = sb.ToString();
            Increment();
            return row;
        }

        private static string Encode(string value) {
            var builder = new StringBuilder();
            for (var i = 0; i < value.Length; i++) {
                var ch = value[i];
                if (ch <= '>') {
                    switch (ch) {
                        case '<':
                            builder.Append("&lt;");
                            break;
                        case '>':
                            builder.Append("&gt;");
                            break;
                        case '"':
                            builder.Append("&quot;");
                            break;
                        case '\'':
                            builder.Append("&#39;");
                            break;
                        case '&':
                            builder.Append("&amp;");
                            break;
                        default:
                            builder.Append(ch);
                            break;
                    }
                } else {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }
    }
}
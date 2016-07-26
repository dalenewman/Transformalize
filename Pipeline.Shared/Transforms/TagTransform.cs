using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class TagTransform : BaseTransform, ITransform {

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

        private readonly Field _contentField;
        private readonly List<TagAttribute> _attributes = new List<TagAttribute>();

        public TagTransform(IContext context) : base(context) {

            if (Context.Transform.Class == string.Empty && Context.Field.Class != string.Empty) {
                Context.Transform.Class = Context.Field.Class;
            }

            if (Context.Transform.Style == string.Empty && Context.Field.Style != string.Empty) {
                Context.Transform.Style = Context.Field.Style;
            }

            var input = MultipleInput();
            _attributes.Add(new TagAttribute(input, "href", Context.Transform.HRef));
            _attributes.Add(new TagAttribute(input, "class", Context.Transform.Class));
            _attributes.Add(new TagAttribute(input, "title", Context.Transform.Title));
            _attributes.Add(new TagAttribute(input, "style", Context.Transform.Style));

            _contentField = input.First();

            if (!Context.Field.Raw) {
                Context.Field.Raw = true;
            }

            if (Context.Field.Length != "max") {
                var calculatedLength = _attributes.Sum(a => a.Length()) + Context.Transform.Tag.Length + 5;  // 5 = <></>
                if (calculatedLength > Convert.ToInt32(Context.Field.Length)) {
                    Context.Warn($"The calculated length of {Context.Field.Alias} is {calculatedLength}, but it's length is set to {Context.Field.Length}.  Truncation may occur.  You need to set the length so it can accomadate tag characters, tag name, attributes, and the field's content.");
                    Context.Field.Length = (calculatedLength + 64).ToString();
                }
            }
        }

        public override IRow Transform(IRow row) {
            var sb = new StringBuilder();

            // open

            sb.AppendFormat("<{0}", Context.Transform.Tag);

            // attributes
            foreach (var attribute in _attributes) {
                attribute.Append(sb, row);
            }

            sb.Append(">");

            // content
            sb.Append(Encode(row[_contentField].ToString()));

            // close
            sb.AppendFormat("</{0}>", Context.Transform.Tag);

            row[Context.Field] = sb.ToString();
            Increment();
            return row;
        }

        static string Encode(string value) {
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
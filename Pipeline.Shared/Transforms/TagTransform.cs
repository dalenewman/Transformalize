using System.Linq;
using System.Text;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class TagTransform : BaseTransform, ITransform {
        private Field[] _input;
        private readonly bool _linkIsField;
        private readonly Field _contentField;
        private readonly Field _linkField;
        private readonly bool _hasHRef;
        private readonly bool _hasClass;
        private readonly bool _hasTitle;
        private readonly bool _hasStyle;
        private readonly Field _classField;
        private readonly Field _titleField;
        private readonly Field _styleField;
        private readonly bool _classIsField;
        private readonly bool _titleIsField;
        private readonly bool _styleIsField;

        public TagTransform(IContext context) : base(context) {
            _input = MultipleInput();

            _hasHRef = Context.Transform.HRef != string.Empty;
            _hasClass = Context.Transform.Class != string.Empty;
            _hasTitle = Context.Transform.Title != string.Empty;
            _hasStyle = Context.Transform.Style != string.Empty;

            _contentField = _input.First();
            _linkField = _hasHRef ? _input.FirstOrDefault(f => f.Alias == Context.Transform.HRef) : null;
            _classField = _hasClass ? _input.FirstOrDefault(f => f.Alias == Context.Transform.Class) : null;
            _titleField = _hasTitle ? _input.FirstOrDefault(f => f.Alias == Context.Transform.Title) : null;
            _styleField = _hasStyle ? _input.FirstOrDefault(f => f.Alias == Context.Transform.Style) : null;

            _linkIsField = _linkField != null;
            _classIsField = _classField != null;
            _titleIsField = _titleField != null;
            _styleIsField = _styleField != null;

        }

        public IRow Transform(IRow row) {
            var sb = new StringBuilder();

            // open

            sb.AppendFormat("<{0}", Context.Transform.Tag);

            // attributes
            AddAttribute(sb, _hasHRef, _linkIsField, "href", _linkField, Context.Transform.HRef, row);
            AddAttribute(sb, _hasClass, _classIsField, "class", _classField, Context.Transform.Class, row);
            AddAttribute(sb, _hasStyle, _styleIsField, "style", _styleField, Context.Transform.Style, row);
            AddAttribute(sb, _hasTitle, _titleIsField,"title", _titleField, Context.Transform.Title, row);

            sb.Append(">");

            // content
            sb.Append(Encode(row[_contentField].ToString()));

            // close

            sb.AppendFormat("</{0}>", Context.Transform.Tag);

            row[Context.Field] = sb.ToString();
            Increment();
            return row;
        }

        static void AddAttribute(StringBuilder sb, bool has, bool isField, string name, IField field, string text, IRow row) {
            if (!has) return;
            sb.Append(' ');
            sb.Append(name);
            sb.Append("=\"");
            sb.Append(isField ? Encode(row[field].ToString()) : Encode(text));
            sb.Append("\"");
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
using System;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Desktop.Actions;
using Transformalize.Transforms;

namespace Transformalize.Desktop.Transforms {
    public class WebTransform : BaseTransform {
        private readonly Func<IRow, Configuration.Action> _getAction;
        private readonly Func<Configuration.Action, ActionResponse> _getResponse;
        private readonly Field _input;

        public WebTransform(IContext context) : base(context, "string") {
            Field url;
            _input = SingleInput();

            if (_input.Equals(Context.Field)) {
                // the url in this field will be transformed inline a web response
                _getAction = row => new Configuration.Action { Type = "web", Url = row[Context.Field] as string, Method = context.Transform.WebMethod, Body = context.Transform.Body };
            } else {
                if (context.Entity.TryGetField(context.Transform.Url, out url)) {
                    // url referenced a field
                    _getAction = row => new Configuration.Action { Type = "web", Url = row[url] as string, Method = context.Transform.WebMethod, Body = context.Transform.Body };
                } else {
                    if (context.Transform.Url != string.Empty) {
                        // url is literal url
                        _getAction = row => new Configuration.Action { Type = "web", Url = context.Transform.Url, Method = context.Transform.WebMethod, Body = context.Transform.Body };
                    } else {
                        // the url as copied in via a parameter (aka copy(url))
                        _getAction = row => new Configuration.Action { Type = "web", Url = row[_input] as string, Method = context.Transform.WebMethod, Body = context.Transform.Body };
                    }
                }
            }

            if (context.Transform.WebMethod == "GET") {
                _getResponse = WebAction.Get;
            } else {
                _getResponse = WebAction.Post;
            }
        }

        public override IRow Transform(IRow row) {
            var response = _getResponse(_getAction(row));
            Increment();
            row[Context.Field] = response.Message;
            return row;
        }
    }
}

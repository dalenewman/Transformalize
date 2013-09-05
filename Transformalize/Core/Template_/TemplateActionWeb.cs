using System.Net;

namespace Transformalize.Core.Template_
{
    public class TemplateActionWeb : TemplateActionHandler
    {
        public override void Handle(TemplateAction action)
        {
            var method = action.Method.ToLower();
            if (!string.IsNullOrEmpty(action.Url))
            {
                var response = method == "post" ? Web.Post(action.Url, string.Empty) : Web.Get(action.Url);
                if (response.Code == HttpStatusCode.OK)
                {
                    Log.Info("Made web request to {0}.", action.Url);
                }
                else
                {
                    Log.Warn("Web request to {0} returned {1}.", action.Url, response.Code);
                }
            }
            else
            {
                Log.Warn("Missing url for web action.");
            }
        }
    }
}
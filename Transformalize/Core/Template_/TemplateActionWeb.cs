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
                    Log.Info("{0} | Made web request to {1}.", action.ProcessName, action.Url);
                }
                else
                {
                    Log.Warn("{0} | Web request to {1} returned {2}.", action.ProcessName, action.Url, response.Code);
                }
            }
            else
            {
                Log.Warn("{0} | Missing url for web action.", action.ProcessName);
            }
        }
    }
}
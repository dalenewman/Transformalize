using Transformalize.Libs.NLog;

namespace Transformalize.Core.Template_
{
    public abstract class TemplateActionHandler
    {
        protected Logger Log = LogManager.GetCurrentClassLogger();
        public abstract void Handle(TemplateAction action);
    }
}
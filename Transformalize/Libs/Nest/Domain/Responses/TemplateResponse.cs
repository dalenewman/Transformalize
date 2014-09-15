using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Mapping.Types;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface ITemplateResponse : IResponse
	{
		string Name { get; }
		TemplateMapping TemplateMapping { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TemplateResponse : BaseResponse, ITemplateResponse
	{
		public string Name { get; internal set; }
		
		public TemplateMapping TemplateMapping { get; internal set; }
	}
}

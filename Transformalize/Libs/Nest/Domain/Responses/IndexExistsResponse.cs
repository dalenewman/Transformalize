using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.Response;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IExistsResponse : IResponse
	{
		bool Exists { get; }
	}

	[JsonObject]
	public class ExistsResponse : BaseResponse, IExistsResponse
	{
		internal ExistsResponse(IElasticsearchResponse connectionStatus)
		{
			this.IsValid =connectionStatus.Success || connectionStatus.HttpStatusCode == 404;
			this.Exists = connectionStatus.Success & connectionStatus.HttpStatusCode == 200;
		}
		public ExistsResponse()
		{
			this.IsValid = false;
			this.Exists = false;
		}

		public bool Exists { get; internal set; }
	}
}

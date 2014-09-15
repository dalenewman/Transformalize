using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IClusterGetSettingsResponse : IResponse
	{
		[JsonProperty(PropertyName = "persistent")]
		IDictionary<string, object> Persistent { get; set; }

		[JsonProperty(PropertyName = "transient")]
		IDictionary<string, object> Transient { get; set; }
	}

	public class ClusterGetSettingsResponse : BaseResponse, IClusterGetSettingsResponse
	{
		public IDictionary<string, object> Persistent { get; set; }
		public IDictionary<string, object> Transient { get; set; }
	}
}
using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Repository
{
	public interface IRepository
	{

		[JsonProperty("settings")]
		IDictionary<string, object> Settings { get; set; }

		[JsonProperty("type")]
		string Type { get; }
	}
}
using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Stats;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface INodeInfoResponse : IResponse
	{
		string ClusterName { get; }
		Dictionary<string, NodeInfo> Nodes { get; }
	}

	[JsonObject]
	public class NodeInfoResponse : BaseResponse, INodeInfoResponse
	{
		public NodeInfoResponse()
		{
			this.IsValid = true;
		}

		[JsonProperty(PropertyName = "cluster_name")]
		public string ClusterName { get; internal set; }

		[JsonProperty(PropertyName = "nodes")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, NodeInfo> Nodes { get; set; }
	}
}

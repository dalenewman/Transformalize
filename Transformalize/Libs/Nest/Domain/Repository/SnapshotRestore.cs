using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Domain.Repository
{
	public class SnapshotRestore
	{
		[JsonProperty("snapshot")]
		public string Name { get; internal set;  }
		[JsonProperty("indices")]
		public IEnumerable<string> Indices { get; internal set; }
		
		[JsonProperty("shards")]
		public ShardsMetaData Shards { get; internal set;  }
	}
}
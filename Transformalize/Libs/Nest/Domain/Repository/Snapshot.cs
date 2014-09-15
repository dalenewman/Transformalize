using System;
using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Domain.Repository
{
	public class Snapshot
	{
		[JsonProperty("snapshot")]
		public string Name { get; internal set;  }
		[JsonProperty("indices")]
		public IEnumerable<string> Indices { get; internal set; }

		[JsonProperty("state")]
		public string State { get; internal set; }
		[JsonProperty("start_time")]
		public DateTime StartTime { get; internal set;  }
		[JsonProperty("start_time_in_millis")]
		public long StartTimeInMilliseconds { get; internal set;  }
		[JsonProperty("end_time")]
		public DateTime EndTime { get; internal set;  }
		[JsonProperty("end_time_in_millis")]
		public long EndTimeInMilliseconds { get; internal set;  }
		[JsonProperty("duration_in_millis")]
		public long DurationInMilliseconds { get; internal set;  }
		[JsonProperty("failures")]
		public IEnumerable<string> Failures { get; internal set;  }
		[JsonProperty("shards")]
		public ShardsMetaData Shards { get; internal set;  }
	}
}
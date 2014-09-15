using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Repository;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IGetSnapshotResponse : IResponse
	{
		[JsonProperty("snapshot")]
		IEnumerable<Snapshot> Snapshots { get; set; }
	}

	[JsonObject]
	public class GetSnapshotResponse : BaseResponse, IGetSnapshotResponse
	{

		[JsonProperty("snapshots")]
		public IEnumerable<Snapshot> Snapshots { get; set; }

	}
}

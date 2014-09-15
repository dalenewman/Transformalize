using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Repository;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IRestoreResponse : IResponse
	{

		[JsonProperty("snapshot")]
		SnapshotRestore Snapshot { get; set; }
	}

	[JsonObject]
	public class RestoreResponse : BaseResponse, IRestoreResponse
	{

		[JsonProperty("snapshot")]
		public SnapshotRestore Snapshot { get; set; }

	}
}

using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.Domain.Mapping.Types
{
	[JsonObject(MemberSerialization.OptIn)]
	public class NestedObjectMapping : ObjectMapping
	{
		[JsonProperty("type")]
		public override TypeNameMarker Type
		{
			get { return new TypeNameMarker { Name = "nested" }; }
		}

		[JsonProperty("include_in_parent")]
		public bool? IncludeInParent { get; set; }

		[JsonProperty("include_in_root")]
		public bool? IncludeInRoot { get; set; }

	}
}
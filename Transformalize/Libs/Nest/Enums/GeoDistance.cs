using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum GeoDistance
	{
		[EnumMember(Value = "sloppy_arc")]
		SloppyArc,
		[EnumMember(Value = "arc")]
		Arc,
		[EnumMember(Value = "plane")]
		Plane
	}
}
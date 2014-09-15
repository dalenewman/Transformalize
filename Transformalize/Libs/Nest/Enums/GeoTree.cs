using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum GeoTree
	{
		[EnumMember(Value = "geohash")]
		Geohash,
		[EnumMember(Value = "quadtree")]
		Quadtree
	}
}

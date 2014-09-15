using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Geometry
{
	public interface IGeoShape
	{
		[JsonProperty("type")]
		string Type { get; }
	}

	public abstract class GeoShape
	{
		public GeoShape(string type)
		{
			this.Type = type;
		}

		public string Type { get; protected set; }
	}
}

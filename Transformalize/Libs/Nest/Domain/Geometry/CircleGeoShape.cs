using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Geometry
{
	public interface ICircleGeoShape : IGeoShape
	{
		[JsonProperty("coordinates")]
		IEnumerable<double> Coordinates { get; set; }

		[JsonProperty("radius")]
		string Radius { get; set; }
	}

	public class CircleGeoShape : GeoShape, ICircleGeoShape
	{
		public CircleGeoShape() : this(null) { }

		public CircleGeoShape(IEnumerable<double> coordinates)
			: base("circle")
		{
			this.Coordinates = coordinates ?? new List<double>();
		}

		public IEnumerable<double> Coordinates { get; set; }

		public string Radius { get; set; }
	}
}

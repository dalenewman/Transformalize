using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Geometry
{
	public interface IPolygonGeoShape : IGeoShape
	{
		[JsonProperty("coordinates")]
		IEnumerable<IEnumerable<IEnumerable<double>>> Coordinates { get; set; }
	}

	public class PolygonGeoShape : GeoShape, IPolygonGeoShape
	{
		public PolygonGeoShape() : this(null) { }

		public PolygonGeoShape(IEnumerable<IEnumerable<IEnumerable<double>>> coordinates) 
			: base("polygon") 
		{
			this.Coordinates = coordinates ?? new List<List<List<double>>>();
		}

		public IEnumerable<IEnumerable<IEnumerable<double>>> Coordinates { get; set; }
	}
}

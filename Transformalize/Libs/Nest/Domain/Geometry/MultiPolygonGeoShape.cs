using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Geometry
{
	public interface IMultiPolygonGeoShape : IGeoShape
	{
		[JsonProperty("coordinates")]
		IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>> Coordinates { get; set; }
	}

	public class MultiPolygonGeoShape : GeoShape, IMultiPolygonGeoShape
	{
		public MultiPolygonGeoShape() : this(null) { }

		public MultiPolygonGeoShape(IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>> coordinates) 
			: base("multipolygon") 
		{
			this.Coordinates = coordinates ?? new List<List<List<List<double>>>>();
		}

		public IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>> Coordinates { get; set; }
	}
}

using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Geometry
{
	public interface IMultiPointGeoShape : IGeoShape
	{
		[JsonProperty("coordinates")]
		IEnumerable<IEnumerable<double>> Coordinates { get; set; }
	}

	public class MultiPointGeoShape : GeoShape, IMultiPointGeoShape
	{
		public MultiPointGeoShape() : this(null) { }

		public MultiPointGeoShape(IEnumerable<IEnumerable<double>> coordinates) 
			: base("multipoint")
		{
			this.Coordinates = coordinates ?? new List<List<double>>();
		}

		public IEnumerable<IEnumerable<double>> Coordinates { get; set; }
	}
}

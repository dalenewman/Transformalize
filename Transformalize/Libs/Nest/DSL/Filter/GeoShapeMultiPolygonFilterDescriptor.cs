using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Geometry;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.DSL.Filter
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IGeoShapeMultiPolygonFilter : IGeoShapeBaseFilter
	{
		[JsonProperty("shape")]
		IMultiPolygonGeoShape Shape { get; set; }
	}

	public class GeoShapeMultiPolygonFilter : PlainFilter, IGeoShapeMultiPolygonFilter
	{
		protected internal override void WrapInContainer(IFilterContainer container)
		{
			container.GeoShape = this;
		}

		public PropertyPathMarker Field { get; set; }

		public IMultiPolygonGeoShape Shape { get; set; }
	}

	public class GeoShapeMultiPolygonFilterDescriptor : FilterBase, IGeoShapeMultiPolygonFilter
	{
		IGeoShapeMultiPolygonFilter Self { get { return this; } }

		bool IFilter.IsConditionless
		{
			get
			{
				return this.Self.Shape == null || !this.Self.Shape.Coordinates.HasAny();
			}
		}

		PropertyPathMarker IFieldNameFilter.Field { get; set; }
		IMultiPolygonGeoShape IGeoShapeMultiPolygonFilter.Shape { get; set; }

		public GeoShapeMultiPolygonFilterDescriptor Coordinates(IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>> coordinates)
		{
			if (this.Self.Shape == null)
				this.Self.Shape = new MultiPolygonGeoShape();
			this.Self.Shape.Coordinates = coordinates;
			return this;
		}
	}

}

using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Geometry;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.DSL.Filter
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IGeoShapeMultiPointFilter : IGeoShapeBaseFilter
	{
		[JsonProperty("shape")]
		IMultiPointGeoShape Shape { get; set; }
	}

	public class GeoShapeMultiPointFilter : PlainFilter, IGeoShapeMultiPointFilter
	{
		protected internal override void WrapInContainer(IFilterContainer container)
		{
			container.GeoShape = this;
		}

		public PropertyPathMarker Field { get; set; }

		public IMultiPointGeoShape Shape { get; set; }
	}

	public class GeoShapeMultiPointFilterDescriptor : FilterBase, IGeoShapeMultiPointFilter
	{
		IGeoShapeMultiPointFilter Self { get { return this; } }

		bool IFilter.IsConditionless
		{
			get
			{
				return this.Self.Shape == null || !this.Self.Shape.Coordinates.HasAny();
			}
		}

		PropertyPathMarker IFieldNameFilter.Field { get; set; }
		IMultiPointGeoShape IGeoShapeMultiPointFilter.Shape { get; set; }

		public GeoShapeMultiPointFilterDescriptor Coordinates(IEnumerable<IEnumerable<double>> coordinates)
		{
			if (this.Self.Shape == null)
				this.Self.Shape = new MultiPointGeoShape();
			this.Self.Shape.Coordinates = coordinates;
			return this;
		}
	}

}

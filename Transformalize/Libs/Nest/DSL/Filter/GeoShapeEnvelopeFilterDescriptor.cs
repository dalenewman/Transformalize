using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Geometry;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.DSL.Filter
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IGeoShapeBaseFilter : IFieldNameFilter
	{
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IGeoShapeEnvelopeFilter : IGeoShapeBaseFilter
	{
		[JsonProperty("shape")]
		IEnvelopeGeoShape Shape { get; set; }
	}

	public class GeoShapeEnvelopeFilter : PlainFilter, IGeoShapeEnvelopeFilter
	{
		protected internal override void WrapInContainer(IFilterContainer container)
		{
			container.GeoShape = this;
		}

		public PropertyPathMarker Field { get; set; }

		public IEnvelopeGeoShape Shape { get; set; }
	}

	public class GeoShapeEnvelopeFilterDescriptor : FilterBase, IGeoShapeEnvelopeFilter
	{
		IGeoShapeEnvelopeFilter Self { get { return this; } }

		bool IFilter.IsConditionless
		{
			get
			{
				return this.Self.Shape == null || !this.Self.Shape.Coordinates.HasAny();
			}
		}

		PropertyPathMarker IFieldNameFilter.Field { get; set; }
		IEnvelopeGeoShape IGeoShapeEnvelopeFilter.Shape { get; set; }

		public GeoShapeEnvelopeFilterDescriptor Coordinates(IEnumerable<IEnumerable<double>> coordinates)
		{
			if (this.Self.Shape == null)
				this.Self.Shape = new EnvelopeGeoShape();
			this.Self.Shape.Coordinates = coordinates;
			return this;
		}
	}

}

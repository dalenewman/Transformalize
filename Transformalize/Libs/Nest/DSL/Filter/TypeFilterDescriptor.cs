using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Resolvers;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Filter
{
	[JsonConverter(typeof(ReadAsTypeConverter<TypeFilterDescriptor>))]
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface ITypeFilter : IFilter
	{
		[JsonProperty(PropertyName = "value")]
		TypeNameMarker Value { get; set; }
	}

	public class TypeFilter : PlainFilter, ITypeFilter
	{
		protected internal override void WrapInContainer(IFilterContainer container)
		{
			container.Type = this;
		}

		public TypeNameMarker Value { get; set; }
	}

	public class TypeFilterDescriptor : FilterBase, ITypeFilter
	{
		bool IFilter.IsConditionless
		{
			get
			{
				return ((ITypeFilter)this).Value.IsConditionless();
			}

		}

		[JsonProperty(PropertyName = "value")]
		TypeNameMarker ITypeFilter.Value { get; set; }
	}
}

using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Extensions;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Filter
{
	[JsonConverter(typeof(ReadAsTypeConverter<AndFilterDescriptor>))]
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IAndFilter : IFilter
	{
		[JsonProperty("filters", 
			ItemConverterType = typeof(CompositeJsonConverter<ReadAsTypeConverter<FilterContainer>, CustomJsonConverter>))]
		IEnumerable<IFilterContainer> Filters { get; set; }
	}
	
	public class AndFilter : PlainFilter, IAndFilter
	{
		protected internal override void WrapInContainer(IFilterContainer container)
		{
			container.And = this;
		}

		public IEnumerable<IFilterContainer> Filters { get; set; }
	}

	public class AndFilterDescriptor : FilterBase, IAndFilter
	{
		IEnumerable<IFilterContainer> IAndFilter.Filters { get; set; }

		bool IFilter.IsConditionless
		{
			get
			{
				var af = ((IAndFilter)this);
				return !af.Filters.HasAny() || af.Filters.All(f=>f.IsConditionless);
			} 
		}
	}
}

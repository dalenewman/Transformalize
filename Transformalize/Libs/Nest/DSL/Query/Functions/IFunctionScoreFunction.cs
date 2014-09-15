using System;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.DSL.Filter;
using Transformalize.Libs.Nest.Extensions;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<FunctionScoreFunction<object>>))]
	public interface IFunctionScoreFunction
	{
	}
	
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class FunctionScoreFunction<T> : IFunctionScoreFunction 
		where T : class
	{
		[JsonProperty(PropertyName = "filter")]
		internal FilterContainer FilterDescriptor { get; set; }

		public FunctionScoreFunction<T> Filter(Func<FilterDescriptor<T>, FilterContainer> filterSelector)
		{
			filterSelector.ThrowIfNull("filterSelector");
			var filter = new FilterDescriptor<T>();
			var f = filterSelector(filter);

			this.FilterDescriptor = f;
			return this;
		}
	}
}
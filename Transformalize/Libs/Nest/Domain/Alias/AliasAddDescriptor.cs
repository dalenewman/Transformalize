using System;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.DSL.Filter;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.Domain.Alias
{
	
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IAliasAddAction : IAliasAction
	{
		[JsonProperty("add")]
		AliasAddOperation Add { get; set; }
	}

	public class AliasAddAction : IAliasAddAction
	{
		public AliasAddOperation Add { get; set; }
	}

	public class AliasAddDescriptor : IAliasAddAction
	{
		private IAliasAddAction Self { get { return this; } }

		public AliasAddDescriptor()
		{
			Self.Add = new AliasAddOperation();
		}

		AliasAddOperation IAliasAddAction.Add { get; set; }

		public AliasAddDescriptor Index(string index)
		{
			Self.Add.Index = index;
			return this;
		}
		public AliasAddDescriptor Index(Type index)
		{
			Self.Add.Index = index;
			return this;
		}
		public AliasAddDescriptor Index<T>() where T : class
		{
			Self.Add.Index = typeof(T);
			return this;
		}
		public AliasAddDescriptor Alias(string alias)
		{
			Self.Add.Alias = alias;
			return this;
		}
		public AliasAddDescriptor Routing(string routing)
		{
			Self.Add.Routing = routing;
			return this;
		}
		public AliasAddDescriptor IndexRouting(string indexRouting)
		{
			Self.Add.IndexRouting = indexRouting;
			return this;
		}
		public AliasAddDescriptor SearchRouting(string searchRouting)
		{
			Self.Add.SearchRouting = searchRouting;
			return this;
		}
		public AliasAddDescriptor Filter<T>(Func<FilterDescriptor<T>, FilterContainer> filterSelector)
			where T : class
		{
			filterSelector.ThrowIfNull("filterSelector");

			Self.Add.FilterDescriptor = filterSelector(new FilterDescriptor<T>());
			return this;
		}
	}
}
using System;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;
using Transformalize.Libs.Nest.DSL.Query;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface ICountRequest : IQueryPath<CountRequestParameters>
	{
		
		[JsonProperty("query")]
		IQueryContainer Query { get; set; }
	}
	public interface ICountRequest<T> : ICountRequest
		where T : class {}

	internal static class CountPathInfo
	{
		public static void Update(ElasticsearchPathInfo<CountRequestParameters> pathInfo, ICountRequest request)
		{
			var source = request.RequestParameters.GetQueryStringValue<string>("source");
			pathInfo.HttpMethod = !source.IsNullOrEmpty() 
				? PathInfoHttpMethod.GET
				: PathInfoHttpMethod.POST;
		
			pathInfo.HttpMethod = PathInfoHttpMethod.POST;
		}
	}
	
	public partial class CountRequest : QueryPathBase<CountRequestParameters>, ICountRequest
	{
		public IQueryContainer Query { get; set; }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<CountRequestParameters> pathInfo)
		{
			CountPathInfo.Update(pathInfo, this);
		}
	}

	public partial class CountRequest<T> : QueryPathBase<CountRequestParameters, T>, ICountRequest
		where T : class
	{
		public IQueryContainer Query { get; set; }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<CountRequestParameters> pathInfo)
		{
			CountPathInfo.Update(pathInfo, this);
		}
	}
	
	[DescriptorFor("Count")]
	public partial class CountDescriptor<T> : QueryPathDescriptorBase<CountDescriptor<T>, CountRequestParameters, T>, ICountRequest
		where T : class
	{
		private ICountRequest Self { get { return this; } }

		IQueryContainer ICountRequest.Query { get; set; }

		public CountDescriptor<T> Query(Func<QueryDescriptor<T>, QueryContainer> querySelector)
		{
			Self.Query = querySelector(new QueryDescriptor<T>());
			return this;
		}

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<CountRequestParameters> pathInfo)
		{
			CountPathInfo.Update(pathInfo, this);
		}
	}
}

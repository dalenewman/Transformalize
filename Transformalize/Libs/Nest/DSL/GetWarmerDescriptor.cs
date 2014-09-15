using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IGetWarmerRequest : IIndicesOptionalTypesNamePath<GetWarmerRequestParameters> { }

	internal static class GetWarmerPathInfo
	{
		public static void Update(ElasticsearchPathInfo<GetWarmerRequestParameters> pathInfo, IGetWarmerRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}
	
	public partial class GetWarmerRequest : IndicesOptionalTypesNamePathBase<GetWarmerRequestParameters>, IGetWarmerRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<GetWarmerRequestParameters> pathInfo)
		{
			GetWarmerPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesGetWarmer")]
	public partial class GetWarmerDescriptor : IndicesOptionalTypesNamePathDescriptor<GetWarmerDescriptor, GetWarmerRequestParameters>, IGetWarmerRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<GetWarmerRequestParameters> pathInfo)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}
}

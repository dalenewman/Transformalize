using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IClearCacheRequest : IIndicesOptionalPath<ClearCacheRequestParameters> { }

	internal static class ClearCachePathInfo
	{
		public static void Update(ElasticsearchPathInfo<ClearCacheRequestParameters> pathInfo, IClearCacheRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.POST;
		}
	}
	
	public partial class ClearCacheRequest : IndicesOptionalPathBase<ClearCacheRequestParameters>, IClearCacheRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<ClearCacheRequestParameters> pathInfo)
		{
			ClearCachePathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesClearCache")]
	public partial class ClearCacheDescriptor : IndicesOptionalPathDescriptor<ClearCacheDescriptor, ClearCacheRequestParameters>, IClearCacheRequest
	{

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<ClearCacheRequestParameters> pathInfo)
		{
			ClearCachePathInfo.Update(pathInfo, this);
		}
	}
}

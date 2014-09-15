using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IRefreshRequest : IIndicesOptionalPath<RefreshRequestParameters> { }

	internal static class RefreshPathInfo
	{
		public static void Update(ElasticsearchPathInfo<RefreshRequestParameters> pathInfo, IRefreshRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.POST;
		}
	}
	
	public partial class RefreshRequest : IndicesOptionalPathBase<RefreshRequestParameters>, IRefreshRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<RefreshRequestParameters> pathInfo)
		{
			RefreshPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesRefresh")]
	public partial class RefreshDescriptor : IndicesOptionalPathDescriptor<RefreshDescriptor, RefreshRequestParameters>, IRefreshRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<RefreshRequestParameters> pathInfo)
		{
			RefreshPathInfo.Update(pathInfo, this);
		}
	}
}

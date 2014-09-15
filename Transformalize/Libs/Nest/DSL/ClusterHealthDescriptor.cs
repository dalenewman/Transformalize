using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IClusterHealthRequest : IIndicesOptionalPath<ClusterHealthRequestParameters> { }

	internal static class ClusterHealthPathInfo
	{
		public static void Update(ElasticsearchPathInfo<ClusterHealthRequestParameters> pathInfo, IClusterHealthRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}
	
	public partial class ClusterHealthRequest : IndicesOptionalPathBase<ClusterHealthRequestParameters>, IClusterHealthRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<ClusterHealthRequestParameters> pathInfo)
		{
			ClusterHealthPathInfo.Update(pathInfo, this);
		}
	}
	public partial class ClusterHealthDescriptor : IndicesOptionalPathDescriptor<ClusterHealthDescriptor, ClusterHealthRequestParameters>, IClusterHealthRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<ClusterHealthRequestParameters> pathInfo)
		{
			ClusterHealthPathInfo.Update(pathInfo, this);
		}
	}
}

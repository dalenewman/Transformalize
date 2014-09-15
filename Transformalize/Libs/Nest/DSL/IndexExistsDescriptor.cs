using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IIndexExistsRequest : IIndexPath<IndexExistsRequestParameters> { }

	internal static class IndexExistsPathInfo
	{
		public static void Update(ElasticsearchPathInfo<IndexExistsRequestParameters> pathInfo, IIndexExistsRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.HEAD;
		}
	}
	
	public partial class IndexExistsRequest : IndexPathBase<IndexExistsRequestParameters>, IIndexExistsRequest
	{
		public IndexExistsRequest(IndexNameMarker index) : base(index) { }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<IndexExistsRequestParameters> pathInfo)
		{
			IndexExistsPathInfo.Update(pathInfo, this);
		}
	}
	[DescriptorFor("IndicesExists")]
	public partial class IndexExistsDescriptor : IndexPathDescriptorBase<IndexExistsDescriptor, IndexExistsRequestParameters>, IIndexExistsRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<IndexExistsRequestParameters> pathInfo)
		{
			IndexExistsPathInfo.Update(pathInfo, this);
		}
	}
}

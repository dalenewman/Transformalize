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
	public interface ICloseIndexRequest : IIndexPath<CloseIndexRequestParameters> { }

	internal static class CloseIndexPathInfo
	{
		public static void Update(ElasticsearchPathInfo<CloseIndexRequestParameters> pathInfo, ICloseIndexRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.POST;
		}
	}
	
	public partial class CloseIndexRequest : IndexPathBase<CloseIndexRequestParameters>, ICloseIndexRequest
	{
		public CloseIndexRequest(IndexNameMarker index) : base(index) { }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<CloseIndexRequestParameters> pathInfo)
		{
			CloseIndexPathInfo.Update(pathInfo, this);
		}
	}
	[DescriptorFor("IndicesClose")]
	public partial class CloseIndexDescriptor : IndexPathDescriptorBase<CloseIndexDescriptor, CloseIndexRequestParameters>, ICloseIndexRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<CloseIndexRequestParameters> pathInfo)
		{
			CloseIndexPathInfo.Update(pathInfo, this);
		}
	}
}

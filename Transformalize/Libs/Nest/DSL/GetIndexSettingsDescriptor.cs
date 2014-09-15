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
	public interface IGetIndexSettingsRequest : IIndexPath<GetIndexSettingsRequestParameters> { }

	internal static class GetIndexSettingsPathInfo
	{
		public static void Update(ElasticsearchPathInfo<GetIndexSettingsRequestParameters> pathInfo, IGetIndexSettingsRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}
	
	public partial class GetIndexSettingsRequest : IndexPathBase<GetIndexSettingsRequestParameters>, IGetIndexSettingsRequest
	{
		public GetIndexSettingsRequest(IndexNameMarker index) : base(index) { }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<GetIndexSettingsRequestParameters> pathInfo)
		{
			GetIndexSettingsPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesGetSettings")]
	public partial class GetIndexSettingsDescriptor : IndexPathDescriptorBase<GetIndexSettingsDescriptor, GetIndexSettingsRequestParameters>, IGetIndexSettingsRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<GetIndexSettingsRequestParameters> pathInfo)
		{
			GetIndexSettingsPathInfo.Update(pathInfo, this);
		}
	}
}

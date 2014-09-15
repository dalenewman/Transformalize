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
	public interface IOpenIndexRequest : IIndexPath<OpenIndexRequestParameters> { }

	internal static class OpenIndexPathInfo
	{
		public static void Update(ElasticsearchPathInfo<OpenIndexRequestParameters> pathInfo, IOpenIndexRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.POST;
		}
	}
	
	public partial class OpenIndexRequest : IndexPathBase<OpenIndexRequestParameters>, IOpenIndexRequest
	{
		public OpenIndexRequest(IndexNameMarker index) : base(index) { }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<OpenIndexRequestParameters> pathInfo)
		{
			OpenIndexPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesOpen")]
	public partial class OpenIndexDescriptor : IndexPathDescriptorBase<OpenIndexDescriptor, OpenIndexRequestParameters>, IOpenIndexRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<OpenIndexRequestParameters> pathInfo)
		{
			OpenIndexPathInfo.Update(pathInfo, this);
		}
	}
}

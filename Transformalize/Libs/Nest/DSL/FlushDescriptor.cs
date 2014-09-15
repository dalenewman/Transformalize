using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IFlushRequest : IIndicesOptionalExplicitAllPath<FlushRequestParameters> { }

	internal static class FlushPathInfo
	{
		public static void Update(ElasticsearchPathInfo<FlushRequestParameters> pathInfo, IFlushRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.POST;
		}
	}
	
	public partial class FlushRequest : IndicesOptionalExplicitAllPathBase<FlushRequestParameters>, IFlushRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<FlushRequestParameters> pathInfo)
		{
			FlushPathInfo.Update(pathInfo, this);
		}
	}
	[DescriptorFor("IndicesFlush")]
	public partial class FlushDescriptor : IndicesOptionalExplicitAllPathDescriptor<FlushDescriptor, FlushRequestParameters>, IFlushRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<FlushRequestParameters> pathInfo)
		{
			FlushPathInfo.Update(pathInfo, this);
		}
	}
}

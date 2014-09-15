using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	public interface ISegmentsRequest : IIndicesOptionalPath<SegmentsRequestParameters> { }

	internal static class SegmentsPathInfo
	{
		public static void Update(IConnectionSettingsValues settings, ElasticsearchPathInfo<SegmentsRequestParameters> pathInfo)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}

	public partial class SegmentsRequest : IndicesOptionalPathBase<SegmentsRequestParameters>, ISegmentsRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<SegmentsRequestParameters> pathInfo)
		{
			SegmentsPathInfo.Update(settings, pathInfo);
		}
	}
	
	[DescriptorFor("IndicesSegments")]
	public partial class SegmentsDescriptor 
		: IndicesOptionalPathDescriptor<SegmentsDescriptor, SegmentsRequestParameters>, ISegmentsRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<SegmentsRequestParameters> pathInfo)
		{
			SegmentsPathInfo.Update(settings, pathInfo);
		}
	}
}

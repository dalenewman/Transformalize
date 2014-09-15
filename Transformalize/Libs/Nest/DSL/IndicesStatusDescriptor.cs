using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IIndicesStatusRequest : IIndicesOptionalPath<IndicesStatusRequestParameters> { }

	internal static class IndicesStatusPathInfo
	{
		public static void Update(ElasticsearchPathInfo<IndicesStatusRequestParameters> pathInfo, IIndicesStatusRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}
	
	public partial class IndicesStatusRequest : IndicesOptionalPathBase<IndicesStatusRequestParameters>, IIndicesStatusRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<IndicesStatusRequestParameters> pathInfo)
		{
			IndicesStatusPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesStatus")]
	public partial class IndicesStatusDescriptor : IndicesOptionalPathDescriptor<IndicesStatusDescriptor, IndicesStatusRequestParameters>, IIndicesStatusRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<IndicesStatusRequestParameters> pathInfo)
		{
			IndicesStatusPathInfo.Update(pathInfo, this);
		}
	}
}

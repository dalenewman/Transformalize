using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IGetTemplateRequest : INamePath<GetTemplateRequestParameters> { }

	internal static class GetTemplatePathInfo
	{
		public static void Update(ElasticsearchPathInfo<GetTemplateRequestParameters> pathInfo, IGetTemplateRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}
	
	public partial class GetTemplateRequest : NamePathBase<GetTemplateRequestParameters>, IGetTemplateRequest
	{
		public GetTemplateRequest(string name) : base(name)
		{
		}

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<GetTemplateRequestParameters> pathInfo)
		{
			GetTemplatePathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesGetTemplate")]
	public partial class GetTemplateDescriptor : NamePathDescriptor<GetTemplateDescriptor, GetTemplateRequestParameters>, IGetTemplateRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<GetTemplateRequestParameters> pathInfo)
		{
			GetTemplatePathInfo.Update(pathInfo, this);
		}
		
	}
}

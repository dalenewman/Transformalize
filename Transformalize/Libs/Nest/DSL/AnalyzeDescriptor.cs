using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IAnalyzeRequest : IIndicesOptionalPath<AnalyzeRequestParameters>
	{
	}

	internal static class AnalyzePathInfo
	{
		public static void Update(ElasticsearchPathInfo<AnalyzeRequestParameters> pathInfo, IAnalyzeRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.POST;
		}
	}
	
	public partial class AnalyzeRequest : IndicesOptionalPathBase<AnalyzeRequestParameters>, IAnalyzeRequest
	{
		public AnalyzeRequest(string textToAnalyze)
		{
			this.Text = textToAnalyze;
		}


		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<AnalyzeRequestParameters> pathInfo)
		{
			AnalyzePathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesAnalyze")]
	public partial class AnalyzeDescriptor : IndicesOptionalPathDescriptor<AnalyzeDescriptor, AnalyzeRequestParameters>, IAnalyzeRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<AnalyzeRequestParameters> pathInfo)
		{
			AnalyzePathInfo.Update(pathInfo, this);
		}
	}
}

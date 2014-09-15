using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class ExpFunction<T> : FunctionScoreDecayFunction<T>
		where T : class
	{
		[JsonProperty(PropertyName = "exp")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		internal IDictionary<PropertyPathMarker, FunctionScoreDecayFieldDescriptor> _ExpDescriptor { get; set; }

		public ExpFunction(Expression<Func<T, object>> objectPath, Action<FunctionScoreDecayFieldDescriptor> descriptorBuilder)
		{
			_ExpDescriptor = new Dictionary<PropertyPathMarker, FunctionScoreDecayFieldDescriptor>();

			var descriptor = new FunctionScoreDecayFieldDescriptor();
			descriptorBuilder(descriptor);
			_ExpDescriptor[objectPath] = descriptor;
		}

		public ExpFunction(string field, Action<FunctionScoreDecayFieldDescriptor> descriptorBuilder)
		{
			_ExpDescriptor = new Dictionary<PropertyPathMarker, FunctionScoreDecayFieldDescriptor>();

			var descriptor = new FunctionScoreDecayFieldDescriptor();
			descriptorBuilder(descriptor);
			_ExpDescriptor[field] = descriptor;
		}
	}
}
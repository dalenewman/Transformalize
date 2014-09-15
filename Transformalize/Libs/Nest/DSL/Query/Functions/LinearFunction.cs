using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class LinearFunction<T> : FunctionScoreDecayFunction<T>
		where T : class
	{
		[JsonProperty(PropertyName = "linear")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		internal IDictionary<PropertyPathMarker, FunctionScoreDecayFieldDescriptor> _LinearDescriptor { get; set; }

		public LinearFunction(Expression<Func<T, object>> objectPath, Action<FunctionScoreDecayFieldDescriptor> descriptorBuilder)
		{
			_LinearDescriptor = new Dictionary<PropertyPathMarker, FunctionScoreDecayFieldDescriptor>();

			var descriptor = new FunctionScoreDecayFieldDescriptor();
			descriptorBuilder(descriptor);
			_LinearDescriptor[objectPath] = descriptor;
		}

		public LinearFunction(string field, Action<FunctionScoreDecayFieldDescriptor> descriptorBuilder)
		{
			_LinearDescriptor = new Dictionary<PropertyPathMarker, FunctionScoreDecayFieldDescriptor>();

			var descriptor = new FunctionScoreDecayFieldDescriptor();
			descriptorBuilder(descriptor);
			_LinearDescriptor[field] = descriptor;
		}
	}
}
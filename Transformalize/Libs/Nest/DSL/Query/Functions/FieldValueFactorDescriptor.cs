using System;
using System.Linq.Expressions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Exception;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class FieldValueFactor<T> : FunctionScoreFunction<T> where T : class
	{
		[JsonProperty(PropertyName = "field_value_factor")]
		internal FieldValueFactorDescriptor<T> _FieldValueFactor { get; set; }

		public FieldValueFactor(Action<FieldValueFactorDescriptor<T>> descriptorBuilder)
		{
			var descriptor = new FieldValueFactorDescriptor<T>();
			descriptorBuilder(descriptor);
			if (descriptor._Field.IsConditionless())
				throw new DslException("Field name not set for field value factor function");

			this._FieldValueFactor = descriptor;
		}
	}

	public class FieldValueFactorDescriptor<T>
	{
		[JsonProperty("field")]
		internal PropertyPathMarker _Field { get; set; }

		[JsonProperty("factor")]
		internal double? _Factor { get; set; }

		[JsonProperty("modifier")]
		[JsonConverter(typeof(StringEnumConverter))]
		internal FieldValueFactorModifier? _Modifier { get; set; }

		public FieldValueFactorDescriptor<T> Field(Expression<Func<T, object>> field)
		{
			this._Field = field;
			return this;
		}

		public FieldValueFactorDescriptor<T> Factor(double factor)
		{
			this._Factor = factor;
			return this;
		}

		public FieldValueFactorDescriptor<T> Modifier(FieldValueFactorModifier modifier)
		{
			this._Modifier = modifier;
			return this;
		}
	}
}

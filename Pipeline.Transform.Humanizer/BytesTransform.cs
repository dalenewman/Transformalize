using System;
using Humanizer;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Humanizer {
    public class BytesTransform : BaseTransform {
        private readonly Field _input;

        public BytesTransform(IContext context) : base(context, "bytesize") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            switch (_input.Type) {
                case "byte":
                    row[Context.Field] = ((byte)row[_input]).Bytes();
                    break;
                case "short":
                case "int16":
                    row[Context.Field] = ((short)row[_input]).Bytes();
                    break;
                case "int":
                case "int32":
                    row[Context.Field] = ((int)row[_input]).Bytes();
                    break;
                case "double":
                    row[Context.Field] = ((double)row[_input]).Bytes();
                    break;
                case "long":
                case "int64":
                    row[Context.Field] = ((long)row[_input]).Bytes();
                    break;
                default:
                    row[Context.Field] = Convert.ToDouble(row[_input]).Bytes();
                    break;
            }
            Increment();
            return row;
        }
    }
}
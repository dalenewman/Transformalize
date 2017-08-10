using Humanizer.Bytes;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Humanizer
{
    public class ByteSizeTransform : BaseTransform {
        private readonly Field _input;
        public ByteSizeTransform(IContext context) : base(context, "double") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            var input = ByteSize.Parse(row[_input].ToString());
            switch (Context.Transform.Units) {
                case "bits":
                    row[Context.Field] = input.Bits;
                    break;
                case "b":
                case "bytes":
                    row[Context.Field] = input.Bytes;
                    break;
                case "kb":
                case "kilobytes":
                    row[Context.Field] = input.Kilobytes;
                    break;
                case "mb":
                case "megabytes":
                    row[Context.Field] = input.Megabytes;
                    break;
                case "gb":
                case "gigabytes":
                    row[Context.Field] = input.Gigabytes;
                    break;
                case "tb":
                case "terabytes":
                    row[Context.Field] = input.Terabytes;
                    break;
                default:
                    row[Context.Field] = 0.0d;
                    break;
            }
            Increment();
            return row;
        }
    }
}
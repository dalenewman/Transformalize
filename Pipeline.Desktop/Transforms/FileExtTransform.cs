using System.IO;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {
    public class FileExtTransform : BaseTransform, ITransform {
        private readonly Field _input;

        public FileExtTransform(IContext context) : base(context) {
            _input = SingleInput();
        }

        public IRow Transform(IRow row) {
            var value = (string)row[_input];
            row[Context.Field] = Path.HasExtension(value) ? Path.GetExtension(value) : string.Empty;
            Increment();
            return row;
        }
    }
}
using System.IO;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {
    public class FileNameTransform : BaseTransform, ITransform {
        private readonly Field _input;

        public FileNameTransform(IContext context) : base(context) {
            _input = SingleInput();
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = Context.Transform.Extension ? Path.GetFileName((string)row[_input]) : Path.GetFileNameWithoutExtension((string)row[_input]);
            Increment();
            return row;
        }
    }
}
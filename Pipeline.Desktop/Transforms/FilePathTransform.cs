using System.IO;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {
    public class FilePathTransform : BaseTransform, ITransform {
        private readonly Field _input;

        public FilePathTransform(IContext context) : base(context) {
            _input = SingleInput();
        }

        public IRow Transform(IRow row) {
            if (Context.Transform.Extension) {
                row[Context.Field] = Path.GetFullPath((string)row[_input]);
            } else {
                var value = (string)row[_input];
                if (Path.HasExtension(value)) {
                    var path = Path.GetFullPath(value);
                    var ext = Path.GetExtension(value);
                    row[Context.Field] = path.Remove(value.Length - ext.Length);
                }
            }
            Increment();
            return row;
        }
    }
}
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Impl {

    public class DefaultRowReader : IRead {

        private readonly IRowFactory _rowFactory;
        private readonly IContext _context;

        public DefaultRowReader(IContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;
        }
        public IEnumerable<IRow> Read() {
            var types = Constants.TypeDefaults();
            var row = _rowFactory.Create();
            foreach (var field in _context.Entity.GetAllFields()) {
                row[field] = field.Convert(field.Default == Constants.DefaultSetting ? types[field.Type] : field.Convert(field.Default));
            }
            yield return row;
        }
    }
}

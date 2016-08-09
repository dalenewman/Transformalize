using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.Web {

    public class WebReader : IRead {

        private readonly InputContext _context;
        private readonly IRowFactory _rowFactory;
        private readonly Field _field;

        public WebReader(InputContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;
            _field = context.Entity.Fields.First(f => f.Input);
        }
        public IEnumerable<IRow> Read() {
            var client = new WebClient();

            var row = _rowFactory.Create();

            try {
                row[_field] = client.DownloadString(_context.Connection.Url);
            } catch (Exception ex) {
                _context.Error(ex.Message);
                _context.Debug(() => ex.StackTrace);
                yield break;
            }

            yield return row;

        }
    }
}
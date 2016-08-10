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
        private readonly WebClient _client;

        public WebReader(InputContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;
            _field = context.Entity.Fields.First(f => f.Input);
            _client = string.IsNullOrEmpty(context.Connection.User) ? new WebClient() : new WebClient { Credentials = new NetworkCredential(_context.Connection.User, _context.Connection.Password) };
            _client.Headers[HttpRequestHeader.Authorization] = $"{"Basic"} {Convert.ToBase64String(System.Text.Encoding.Default.GetBytes($"{_context.Connection.User}:{_context.Connection.Password}"))}";

        }
        public IEnumerable<IRow> Read() {
            var row = _rowFactory.Create();

            try {
                row[_field] = _client.DownloadString(_context.Connection.Url);
            } catch (Exception ex) {
                _context.Error(ex.Message);
                _context.Debug(() => ex.StackTrace);
                yield break;
            }

            yield return row;

        }


    }
}
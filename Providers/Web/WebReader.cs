#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Web {

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
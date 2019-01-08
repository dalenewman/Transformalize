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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Web {

    public class WebCsvReader : IRead {

        private readonly InputContext _context;
        private readonly Regex _regex = new Regex(@"""?\s*,\s*""?", RegexOptions.Compiled);
        private readonly IRowFactory _rowFactory;
        private readonly WebClient _client;

        public WebCsvReader(InputContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;
            _client = string.IsNullOrEmpty(context.Connection.User) ? new WebClient() : new WebClient { Credentials = new NetworkCredential(_context.Connection.User, _context.Connection.Password) };
            _client.Headers[HttpRequestHeader.Authorization] = $"{"Basic"} {Convert.ToBase64String(System.Text.Encoding.Default.GetBytes($"{_context.Connection.User}:{_context.Connection.Password}"))}";

        }

        public IEnumerable<IRow> Read() {

            var stream = _client.OpenRead(_context.Connection.Url);

            if (stream == null) {
                _context.Error("Could not open {0}.", _context.Connection.Url);
                yield break;
            }

            var start = _context.Connection.Start;
            var end = _context.Connection.End;
            var isPageRequest = _context.Entity.IsPageRequest();

            if (isPageRequest) {
                start += ((_context.Entity.Page * _context.Entity.Size) - _context.Entity.Size);
                end = start + _context.Entity.Size;
            }

            using (var reader = new StreamReader(stream)) {
                string line;

                _context.Entity.Hits = 1;

                if (start > 1) {
                    for (var i = 1; i < start; i++) {
                        reader.ReadLine();
                        _context.Entity.Hits++;
                    }
                }

                while ((line = reader.ReadLine()) != null) {
                    if (end > 0 && _context.Entity.Hits >= end) {
                        if (isPageRequest) {
                            _context.Entity.Hits++;
                            continue;
                        }

                        yield break;
                    }

                    _context.Entity.Hits++;
                    var tokens = _regex.Split(line.Trim('"'));
                    if (tokens.Length > 0) {
                        var row = _rowFactory.Create();
                        for (var i = 0; i < _context.InputFields.Length && i < tokens.Length; i++) {
                            var field = _context.InputFields[i];
                            row[field] = tokens[i];
                        }
                        yield return row;
                    }
                }

                if (isPageRequest && start > 1) {
                    _context.Entity.Hits -= (start - 1);
                }
            }
        }
    }
}
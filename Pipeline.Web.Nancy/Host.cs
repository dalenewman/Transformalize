#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Nancy.Hosting.Self;
using Pipeline.Contracts;

namespace Pipeline.Web.Nancy {
    public class Host : IHost {
        private readonly int _port;
        readonly IContext _context;
        readonly NancyHost _host;

        public Host(int port, IContext context, NancyHost host) {
            _port = port;
            _context = context;
            _host = host;
        }

        public void Start() {
            _context.Info("Starting Host: http://localhost:{0}", _port);
            _host.Start();
        }
        public void Stop() {
            _context.Info("Stopping Host: http://localhost:{0}", _port);
            _host.Stop();
        }
    }
}

#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Context;

namespace Transformalize.Providers.Elasticsearch.Ext {
    public static class ElasticExtensions {

        public static string TypeName(this InputContext context) {
            return context.Entity.Name;
        }

        public static string TypeName(this OutputContext context) {
            return context.Entity.Alias.ToLower();
        }

        public static string GetElasticUrl(this Connection cn) {
            if (!string.IsNullOrEmpty(cn.Url)) {
                return cn.Url;
            }
            
            var builder = new UriBuilder(cn.Server.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? cn.Server : $"http{(cn.UseSsl ? "s" : "")}://" + cn.Server);
            if (cn.Port > 0) {
                builder.Port = cn.Port;
            }
            if (cn.Path != string.Empty) {
                builder.Path = cn.Path;
            }
            return builder.ToString();
        }

        public static string GetElasticUrl(this Server s) {
            if (!string.IsNullOrEmpty(s.Url)) {
                return s.Url;
            }
            var builder = new UriBuilder(s.Name.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? s.Name : "http://" + s.Name);
            if (s.Port > 0) {
                builder.Port = s.Port;
            }
            if (s.Path != string.Empty) {
                builder.Path = s.Path;
            }
            return builder.ToString();
        }
    }
}

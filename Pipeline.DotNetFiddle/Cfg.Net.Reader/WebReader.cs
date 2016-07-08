#region license
// Cfg.Net
// Copyright 2015 Dale Newman
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Cfg.Net.Contracts;

namespace Cfg.Net.Reader {
    public class WebReader : IReader {

        public string Read(string url, IDictionary<string,string> parameters, ILogger logger) {

            if (string.IsNullOrEmpty(url)) {
                logger.Error("Your configuration url null or empty.");
                return null;
            }

            try {
                var uri = new Uri(url);
                if (!string.IsNullOrEmpty(uri.Query)) {
                    var newParameters = HttpUtility.ParseQueryString(uri.Query.Substring(1));
                    foreach (var pair in newParameters) {
                        parameters[pair.Key] = pair.Value;
                    }
                }
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";

                using (var response = (HttpWebResponse)request.GetResponse()) {
                    using (var responseStream = response.GetResponseStream()) {
                        if (responseStream == null) {

                        } else {
                            if (response.StatusCode == HttpStatusCode.OK) {
                                return new StreamReader(responseStream).ReadToEnd();
                            }

                            logger.Error("Response code was {0}. {1}", response.StatusCode, response.StatusDescription);
                            return null;
                        }
                    }
                }
            } catch (Exception ex) {
                logger.Error("Can not read url. {0}", ex.Message);
                return null;
            }

            return null;
        }
    }
}
#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Action = Pipeline.Configuration.Action;

namespace Pipeline.Desktop.Actions {
    public class WebAction : IAction {
        private readonly PipelineContext _context;
        private readonly Action _action;

        public WebAction(PipelineContext context, Action action) {
            _context = context;
            _action = action;
        }

        public ActionResponse Execute() {
            _context.Info("Web request to {0}", _action.Url);
            return _action.Method == "get"
                ? Get(_action.Url, _action.TimeOut)
                : Post(_action.Url, _action.TimeOut, _action.Content);
        }

        public static ActionResponse Get(string url, int timeOut) {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = timeOut == 0 ? Timeout.Infinite : timeOut;
            request.KeepAlive = timeOut == 0;

            try {
                using (var response = (HttpWebResponse)request.GetResponse()) {
                    using (var responseStream = response.GetResponseStream()) {
                        if (responseStream == null)
                            return new ActionResponse { Code = (int)response.StatusCode };
                        var reader = new StreamReader(responseStream);
                        return new ActionResponse((int)response.StatusCode, reader.ReadToEnd());
                    }
                }
            } catch (Exception e) {
                return new ActionResponse(500, e.Message);
            }
        }

        public static ActionResponse Post(string url, int timeOut, string postData) {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = timeOut == 0 ? Timeout.Infinite : timeOut;
            request.KeepAlive = timeOut == 0;
            request.ContentType = "application/x-www-form-urlencoded";

            var byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            using (var dataStream = request.GetRequestStream()) {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            try {
                using (var response = (HttpWebResponse)request.GetResponse()) {
                    using (var responseStream = response.GetResponseStream()) {
                        if (responseStream == null)
                            return new ActionResponse((int)response.StatusCode);
                        var reader = new StreamReader(responseStream);
                        return new ActionResponse((int)response.StatusCode, reader.ReadToEnd());
                    }
                }
            } catch (Exception e) {
                return new ActionResponse(500, e.Message);
            }
        }

    }
}
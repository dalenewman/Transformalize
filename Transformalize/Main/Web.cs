#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.IO;
using System.Net;
using System.Text;

namespace Transformalize.Main {
    public class WebResponse {
        public WebResponse() {
            Code = HttpStatusCode.InternalServerError;
            Content = string.Empty;
        }

        public WebResponse(string content) {
            Code = HttpStatusCode.InternalServerError;
            Content = content;
        }

        public WebResponse(HttpStatusCode code, string content) {
            Code = code;
            Content = content;
        }

        public HttpStatusCode Code { get; set; }
        public string Content { get; set; }
    }

    public class Web {

        public static WebResponse Post(string url, string postData) {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
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
                            return new WebResponse();
                        var reader = new StreamReader(responseStream);
                        return new WebResponse(response.StatusCode, reader.ReadToEnd());
                    }
                }
            } catch (Exception e) {
                return new WebResponse(e.Message);
            }
        }

        public static WebResponse Get(string url) {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            try {
                using (var response = (HttpWebResponse)request.GetResponse()) {
                    using (var responseStream = response.GetResponseStream()) {
                        if (responseStream == null)
                            return new WebResponse();
                        var reader = new StreamReader(responseStream);
                        return new WebResponse(response.StatusCode, reader.ReadToEnd());
                    }
                }
            } catch (Exception e) {
                return new WebResponse(e.Message);
            }
        }

        public static void DownloadFile(string url, string fileName) {
            using (var webClient = new WebClient())
                webClient.DownloadFile(url, fileName);
        }
    }
}
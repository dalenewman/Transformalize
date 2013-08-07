using System;
using System.IO;
using System.Net;
using System.Text;

namespace Transformalize.Core {

    public class WebResponse {
        public HttpStatusCode Code { get; set; }
        public string Content { get; set; }

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
    }

    public class Web {

        public static WebResponse Post(string url, string postData) {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            using (var dataStream = request.GetRequestStream())
                dataStream.Write(byteArray, 0, byteArray.Length);

            try {
                using (var response = (HttpWebResponse)request.GetResponse()) {

                    using (var responseStream = response.GetResponseStream()) {
                        if (responseStream == null)
                            return new WebResponse();
                        var reader = new StreamReader(responseStream);
                        return new WebResponse(response.StatusCode, reader.ReadToEnd());
                    }
                }

            }
            catch (Exception e) {
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
            }
            catch (Exception e) {
                return new WebResponse(e.Message);
            }

        }

        public static void DownloadFile(string url, string fileName) {
            using (var webClient = new WebClient())
                webClient.DownloadFile(url, fileName);
        }

    }
}


using System.Collections.Generic;
using System.Net;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main.Parameters;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {
    public class WebOperation : ShouldRunOperation {
        private readonly IParameter _url;
        private readonly int _sleep;
        private readonly IParameter _data;
        private readonly string _webMethod;
        private readonly WebClient _webClient;
        private readonly bool _useParamForData;
        private readonly bool _useParamForUrl;

        public WebOperation(IParameter url, string outKey, int sleep, string webMethod, IParameter data, string contentType)
            : base(string.Empty, outKey) {
            _url = url;
            _sleep = sleep;
            _data = data;
            _webMethod = webMethod.ToUpper();
            _useParamForData = data.Value == null || !data.Value.Equals(Common.DefaultValue);
            _useParamForUrl = !url.HasValue();
            _webClient = new WebClient();
            if (!contentType.Equals(string.Empty)) {
                _webClient.Headers[HttpRequestHeader.ContentType] = contentType;
            }
            Name = "Web (" + outKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var url = (_useParamForUrl ? row[_url.Name] : _url.Value).ToString();
                    if (_webMethod.Equals("GET")) {
                        row[OutKey] = _webClient.DownloadString(url);
                    } else {
                        row[OutKey] = _webClient.UploadString(
                            url,
                            _webMethod,
                            (_useParamForData ? row[_data.Name] : _data.Value).ToString()
                        );
                    }
                    if (_sleep > 0) {
                        System.Threading.Thread.Sleep(_sleep);
                    }
                }
                yield return row;
            }
        }
    }
}
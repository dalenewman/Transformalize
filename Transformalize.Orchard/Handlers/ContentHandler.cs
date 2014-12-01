using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers {
    public static class ApiContentHandler {
        public static string GetErrorContent(string format, ApiRequest request) {
            var empty = format.StartsWith("<") ? string.Empty : "[]";
            return string.Format(
                format,
                request.RequestType.ToString().ToLower(),
                request.Status,
                request.Message,
                request.Stopwatch.ElapsedMilliseconds,
                empty,
                empty,
                empty,
                empty
                );
        }
    }
}
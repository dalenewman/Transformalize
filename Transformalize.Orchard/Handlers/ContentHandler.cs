using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers
{
    public static class ApiContentHandler {
        public static string GetErrorContent(string format, ApiRequest request) {
            return string.Format(
                format,
                request.RequestType.ToString().ToLower(),
                request.Status,
                request.Message,
                request.Stopwatch.ElapsedMilliseconds,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
                );
        }
    }
}
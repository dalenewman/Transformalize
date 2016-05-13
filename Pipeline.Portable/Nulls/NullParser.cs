using System;
using Pipeline.Contracts;

namespace Pipeline.Nulls {
    public class NullParser : IParser {
        public bool Parse(string script, Action<string, object[]> error) {
            return true;
        }
    }
}
#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Transformalize.Libs.fastJSON {
    public class Result {
        public bool IsValid { get { return string.IsNullOrEmpty(Message); } }
        public string Message { get; set; }
        public Result(string message) {
            Message = message;
        }
        public Result() {
            Message = string.Empty;
        }
    }

    /// <summary>
    ///     This is a hack of Fast JSON's JsonParser class, that should validate whether or not the json string is valid (or, can be parsed)
    ///     Spec. details, see http://www.json.org/
    /// </summary>
    internal sealed class JsonValidater {

        private readonly bool _ignorecase;
        private readonly char[] _json;
        private readonly StringBuilder s = new StringBuilder();
        private int index;
        private Token lookAheadToken = Token.None;


        internal JsonValidater(string json, bool ignorecase) {
            this._json = json.ToCharArray();
            _ignorecase = ignorecase;
        }

        public bool TryParse(out object response) {
            var value = ParseValue();
            var error = value as Result;
            if (error == null) {
                response = value;
                return true;
            }
            response = null;
            return false;
        }

        public Result Validate() {
            var result = new Result();
            var response = ParseValue();
            var error = response as Result;
            return error ?? result;
        }

        private object ParseObject() {
            var table = new Dictionary<string, object>();

            ConsumeToken(); // {

            while (true) {
                var la = LookAhead();
                if (la is Result)
                    return la;

                switch ((Token)la) {
                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Curly_Close:
                        ConsumeToken();
                        return table;

                    default: {
                            // name
                            var name = ParseString();
                            if (_ignorecase)
                                name = name.ToLower();

                            // :
                            var nt = NextToken();
                            if (nt is Result)
                                return nt;

                            if ((Token)nt != Token.Colon) {
                                return new Result("Expected colon at index " + index);
                            }

                            // value
                            var value = ParseValue();
                            if (value is Result) {
                                return value;
                            }
                            table[name] = value;
                        }
                        break;
                }
            }
        }

        private object ParseArray() {
            var array = new List<object>();
            ConsumeToken(); // [

            while (true) {
                var la = LookAhead();
                if (la is Result)
                    return la;
                switch ((Token)la) {
                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Squared_Close:
                        ConsumeToken();
                        return array;

                    default:
                        var value = ParseValue();
                        if (value is Result)
                            return value;
                        array.Add(value);
                        break;
                }
            }
        }

        private object ParseValue() {
            var la = LookAhead();
            if (la is Result)
                return la;

            switch ((Token)la) {
                case Token.Number:
                    return ParseNumber();

                case Token.String:
                    return ParseString();

                case Token.Curly_Open:
                    return ParseObject();

                case Token.Squared_Open:
                    return ParseArray();

                case Token.True:
                    ConsumeToken();
                    return true;

                case Token.False:
                    ConsumeToken();
                    return false;

                case Token.Null:
                    ConsumeToken();
                    return null;
            }

            return new Result("Unrecognized token at index" + index);
        }

        private string ParseString() {
            ConsumeToken(); // "

            s.Length = 0;

            var runIndex = -1;

            while (index < _json.Length) {
                var c = _json[index++];

                if (c == '"') {
                    if (runIndex != -1) {
                        if (s.Length == 0)
                            return new string(_json, runIndex, index - runIndex - 1);

                        s.Append(_json, runIndex, index - runIndex - 1);
                    }
                    return s.ToString();
                }

                if (c != '\\') {
                    if (runIndex == -1)
                        runIndex = index - 1;

                    continue;
                }

                if (index == _json.Length)
                    break;

                if (runIndex != -1) {
                    s.Append(_json, runIndex, index - runIndex - 1);
                    runIndex = -1;
                }

                switch (_json[index++]) {
                    case '"':
                        s.Append('"');
                        break;

                    case '\\':
                        s.Append('\\');
                        break;

                    case '/':
                        s.Append('/');
                        break;

                    case 'b':
                        s.Append('\b');
                        break;

                    case 'f':
                        s.Append('\f');
                        break;

                    case 'n':
                        s.Append('\n');
                        break;

                    case 'r':
                        s.Append('\r');
                        break;

                    case 't':
                        s.Append('\t');
                        break;

                    case 'u': {
                            var remainingLength = _json.Length - index;
                            if (remainingLength < 4)
                                break;

                            // parse the 32 bit hex into an integer codepoint
                            var codePoint = ParseUnicode(_json[index], _json[index + 1], _json[index + 2], _json[index + 3]);
                            s.Append((char)codePoint);

                            // skip 4 chars
                            index += 4;
                        }
                        break;
                }
            }

            return "Unexpectedly reached end of string";
        }

        private static uint ParseSingleChar(char c1, uint multipliyer) {
            uint p1 = 0;
            if (c1 >= '0' && c1 <= '9')
                p1 = (uint)(c1 - '0') * multipliyer;
            else if (c1 >= 'A' && c1 <= 'F')
                p1 = (uint)((c1 - 'A') + 10) * multipliyer;
            else if (c1 >= 'a' && c1 <= 'f')
                p1 = (uint)((c1 - 'a') + 10) * multipliyer;
            return p1;
        }

        private static uint ParseUnicode(char c1, char c2, char c3, char c4) {
            var p1 = ParseSingleChar(c1, 0x1000);
            var p2 = ParseSingleChar(c2, 0x100);
            var p3 = ParseSingleChar(c3, 0x10);
            var p4 = ParseSingleChar(c4, 1);

            return p1 + p2 + p3 + p4;
        }

        private object ParseNumber() {
            ConsumeToken();

            // Need to start back one place because the first digit is also a token and would have been consumed
            var startIndex = index - 1;
            var dec = false;
            do {
                if (index == _json.Length)
                    break;
                var c = _json[index];

                if ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E') {
                    if (c == '.' || c == 'e' || c == 'E')
                        dec = true;
                    if (++index == _json.Length)
                        break; //throw new Exception("Unexpected end of string whilst parsing number");
                    continue;
                }
                break;
            } while (true);

            if (dec) {
                var s = new string(_json, startIndex, index - startIndex);
                return double.Parse(s, NumberFormatInfo.InvariantInfo);
            }
            long num;
            return JSON.CreateLong(out num, _json, startIndex, index - startIndex);
        }

        private Object LookAhead() {
            if (lookAheadToken != Token.None)
                return lookAheadToken;

            var value = NextTokenCore();
            if (value is Result)
                return value;

            return lookAheadToken = (Token)value;
        }

        private void ConsumeToken() {
            lookAheadToken = Token.None;
        }

        private Object NextToken() {
            var result = lookAheadToken != Token.None ? lookAheadToken : NextTokenCore();

            lookAheadToken = Token.None;

            return result;
        }

        private Object NextTokenCore() {
            char c;

            // Skip past whitespace
            do {
                c = _json[index];

                if (c > ' ')
                    break;
                if (c != ' ' && c != '\t' && c != '\n' && c != '\r')
                    break;
            } while (++index < _json.Length);

            if (index == _json.Length) {
                return new Result("Reached end of string unexpectedly");
            }

            c = _json[index];

            index++;

            //if (c >= '0' && c <= '9')
            //    return Token.Number;

            switch (c) {
                case '{':
                    return Token.Curly_Open;

                case '}':
                    return Token.Curly_Close;

                case '[':
                    return Token.Squared_Open;

                case ']':
                    return Token.Squared_Close;

                case ',':
                    return Token.Comma;

                case '"':
                    return Token.String;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                case '+':
                case '.':
                    return Token.Number;

                case ':':
                    return Token.Colon;

                case 'f':
                    if (_json.Length - index >= 4 &&
                        _json[index + 0] == 'a' &&
                        _json[index + 1] == 'l' &&
                        _json[index + 2] == 's' &&
                        _json[index + 3] == 'e') {
                        index += 4;
                        return Token.False;
                    }
                    break;

                case 't':
                    if (_json.Length - index >= 3 &&
                        _json[index + 0] == 'r' &&
                        _json[index + 1] == 'u' &&
                        _json[index + 2] == 'e') {
                        index += 3;
                        return Token.True;
                    }
                    break;

                case 'n':
                    if (_json.Length - index >= 3 &&
                        _json[index + 0] == 'u' &&
                        _json[index + 1] == 'l' &&
                        _json[index + 2] == 'l') {
                        index += 3;
                        return Token.Null;
                    }
                    break;
            }

            return new Result("Could not find token at index " + --index);
        }

        private enum Token {
            None = -1, // Used to denote no Lookahead available
            Curly_Open,
            Curly_Close,
            Squared_Open,
            Squared_Close,
            Colon,
            Comma,
            String,
            Number,
            True,
            False,
            Null
        }
    }
}
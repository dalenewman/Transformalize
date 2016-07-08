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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Cfg.Net.Reader {

    // http://stackoverflow.com/questions/20268544/portable-class-library-pcl-version-of-httputility-parsequerystring
    internal sealed class HttpUtility {
        public static Dictionary<string, string> ParseQueryString(string query, bool urlencoded = true) {
            return ToDictionary(new HttpValueCollection(query, urlencoded));
        }

        // this is added by me, I needed a case-less string dictionary
        private static Dictionary<string, string> ToDictionary(IEnumerable<HttpValue> collection) {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in collection) {
                if (result.ContainsKey(item.Key)) {
                    result[item.Key] += "," + item.Value;
                } else {
                    result[item.Key] = item.Value;
                }
            }
            return result;
        }
    }

    internal sealed class HttpValue {
        public HttpValue() {
        }

        public HttpValue(string key, string value) {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }

    internal class HttpValueCollection : Collection<HttpValue> {
        public HttpValueCollection() {
        }

        public HttpValueCollection(string query)
            : this(query, true) {
        }

        public HttpValueCollection(string query, bool urlencoded) {
            if (!string.IsNullOrEmpty(query)) {
                FillFromString(query, urlencoded);
            }
        }

        public string this[string key] {
            get { return this.First(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).Value; }
            set { this.First(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).Value = value; }
        }

        public void Add(string key, string value) {
            Add(new HttpValue(key, value));
        }

        public bool ContainsKey(string key) {
            return this.Any(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
        }

        public string[] GetValues(string key) {
            return
                this.Where(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Value)
                    .ToArray();
        }

        public void Remove(string key) {
            this.Where(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(x => Remove(x));
        }

        public override string ToString() {
            return ToString(true);
        }

        public virtual string ToString(bool urlencoded) {
            return ToString(urlencoded, null);
        }

        public virtual string ToString(bool urlencoded, IDictionary excludeKeys) {
            if (Count == 0) {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            foreach (HttpValue item in this) {
                string key = item.Key;

                if ((excludeKeys == null) || !excludeKeys.Contains(key)) {
                    string value = item.Value;

                    if (urlencoded) {
                        // If .NET 4.5 and above (Thanks @Paya)
                        // key = WebUtility.UrlDecode(key);
                        // If .NET 4.0 use this instead.
                        key = Uri.EscapeDataString(key);
                    }

                    if (stringBuilder.Length > 0) {
                        stringBuilder.Append('&');
                    }

                    stringBuilder.Append((key != null) ? (key + "=") : string.Empty);

                    if (string.IsNullOrEmpty(value))
                        continue;

                    if (urlencoded) {
                        value = Uri.EscapeDataString(value);
                    }

                    stringBuilder.Append(value);
                }
            }

            return stringBuilder.ToString();
        }

        private void FillFromString(string query, bool urlencoded) {
            int num = (query != null) ? query.Length : 0;
            for (int i = 0; i < num; i++) {
                int startIndex = i;
                int num4 = -1;
                while (i < num) {
                    char ch = query[i];
                    if (ch == '=') {
                        if (num4 < 0) {
                            num4 = i;
                        }
                    } else if (ch == '&') {
                        break;
                    }
                    i++;
                }
                string str = null;
                string str2 = null;
                if (num4 >= 0) {
                    str = query.Substring(startIndex, num4 - startIndex);
                    str2 = query.Substring(num4 + 1, (i - num4) - 1);
                } else {
                    str2 = query.Substring(startIndex, i - startIndex);
                }

                // added by me, rather have a key with empty value then an error
                if (str == null) {
                    str = str2;
                    str2 = string.Empty;
                }

                if (urlencoded) {
                    Add(Uri.UnescapeDataString(str), Uri.UnescapeDataString(str2));
                } else {
                    Add(str, str2);
                }

                if ((i == (num - 1)) && (query[i] == '&')) {
                    Add(null, string.Empty);
                }
            }
        }
    }
}
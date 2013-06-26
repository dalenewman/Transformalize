using System.Collections.Generic;
using System.Text;

namespace Transformalize.Transforms {
    public class MapTransform : ITransform {
        private readonly IReadOnlyDictionary<string, object> _equals;
        private readonly IReadOnlyDictionary<string, object> _startsWith;
        private readonly IReadOnlyDictionary<string, object> _endsWith;

        public MapTransform(IList<IReadOnlyDictionary<string, object>> maps) {
            _equals = maps[0];
            _startsWith = maps[1];
            _endsWith = maps[2];
        }

        public void Transform(StringBuilder sb) {
            var sbLen = sb.Length;

            foreach (var key in _equals.Keys) {
                var keyLen = key.Length;
                if (sbLen == keyLen) {
                    for (int i = 0; i < keyLen; i++) {
                        if (sb[i].Equals(key[i])) {
                            if (i == keyLen - 1) {
                                sb.Clear();
                                sb.Append(_equals[key]);
                                goto done;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }
            }

            foreach (var key in _startsWith.Keys) {
                var keyLen = key.Length;
                if (keyLen < sbLen) {
                    for (int i = 0; i < keyLen; i++) {
                        if (sb[i].Equals(key[i])) {
                            if (i == keyLen - 1) {
                                sb.Clear();
                                sb.Append(_startsWith[key]);
                                goto done;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }
            }

            foreach (var key in _endsWith.Keys) {
                var keyLen = key.Length;
                if (keyLen < sbLen) {
                    for (int i = 0; i < keyLen; i++) {
                        if (key[i].Equals(sb[sbLen - keyLen + i])) {
                            if (i == keyLen - 1) {
                                sb.Clear();
                                sb.Append(_endsWith[key]);
                                goto done;
                            }
                        } else {
                            break;
                        }
                    }
                }
            }

            foreach (var key in _equals.Keys) {
                if (key.Equals("*")) {
                    sb.Clear();
                    sb.Append(_equals[key]);
                    goto done;
                }
            }

        done: ;
        }
    }
}
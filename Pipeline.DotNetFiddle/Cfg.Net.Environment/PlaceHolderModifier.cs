using System.Collections.Generic;
using System.Text;
using Cfg.Net.Contracts;

namespace Cfg.Net.Environment {
    public class PlaceHolderModifier : IGlobalModifier {
        private readonly char _placeHolderMarker;
        private readonly char _placeHolderOpen;
        private readonly char _placeHolderClose;

        public PlaceHolderModifier() : this('@', '(', ')') { }

        public PlaceHolderModifier(char placeHolderMarker, char placeHolderOpen, char placeHolderClose) {
            _placeHolderMarker = placeHolderMarker;
            _placeHolderOpen = placeHolderOpen;
            _placeHolderClose = placeHolderClose;
        }

        public object Modify(string name, object value, IDictionary<string, string> parameters)
        {

            var str = value as string;
            if (str == null)
                return value;

                if (parameters.Count == 0 || str.IndexOf(_placeHolderMarker) < 0)
                    return value;

                var builder = new StringBuilder();
                for (var j = 0; j < str.Length; j++) {
                    if (str[j] == _placeHolderMarker && str.Length > j + 1 && str[j + 1] == _placeHolderOpen) {
                        var length = 2;
                        while (str.Length > j + length && str[j + length] != _placeHolderClose) {
                            length++;
                        }
                        if (length > 2) {
                            var key = str.Substring(j + 2, length - 2);
                            if (parameters.ContainsKey(key)) {
                                builder.Append(parameters[key]);
                            } else {
                                builder.AppendFormat("@({0})", key);
                            }
                        }
                        j = j + length;
                    } else {
                        builder.Append(str[j]);
                    }
                }

                return builder.ToString();

        }
    }
}

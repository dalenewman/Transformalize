using System.Collections.Generic;
using System.Globalization;
using Transformalize.Libs.Lucene.Net.Document;

namespace Transformalize.Libs.Sqloogle.Utilities {
    public class Docs {
        public static IDictionary<string, string> DocToDict(Document doc, float score = 0) {
            var dict = new Dictionary<string, string>();
            foreach (var field in doc.GetFields()) {
                if (field.IsStored)
                    dict[field.Name] = doc.Get(field.Name);
            }
            dict["rank"] = score.ToString(CultureInfo.InvariantCulture);
            return dict;
        }


    }
}

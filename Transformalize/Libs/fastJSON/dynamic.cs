using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Transformalize.Libs.fastJSON
{
    internal class DynamicJson : DynamicObject
    {
        public DynamicJson(string json)
        {
            object dictionary = JSON.Instance.Parse(json);
            if (dictionary is IDictionary<string, object>)
                Dictionary = (IDictionary<string, object>) dictionary;
        }

        private DynamicJson(object dictionary)
        {
            if (dictionary is IDictionary<string, object>)
                Dictionary = (IDictionary<string, object>) dictionary;
        }

        private IDictionary<string, object> Dictionary { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Dictionary.TryGetValue(binder.Name, out result) == false)
                if (Dictionary.TryGetValue(binder.Name.ToLower(), out result) == false)
                    return false; // throw new Exception("property not found " + binder.Name);

            if (result is IDictionary<string, object>)
                result = new DynamicJson(result as IDictionary<string, object>);

            else if (result is List<object> && (result as List<object>) is IDictionary<string, object>)
                result =
                    new List<DynamicJson>(
                        (result as List<object>).Select(x => new DynamicJson(x as IDictionary<string, object>)));

            else if (result is List<object>)
                result = result as List<object>;

            return Dictionary.ContainsKey(binder.Name);
        }
    }
}
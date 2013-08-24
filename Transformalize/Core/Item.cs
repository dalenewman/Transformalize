using Transformalize.Core.Process_;

namespace Transformalize.Core
{
    public class Item
    {
        public string Parameter = null;
        public object Value = null;
        public bool UseParameter = false;

        public Item(string parameter, object value)
        {
            if (parameter != string.Empty)
            {
                Parameter = parameter;
                UseParameter = true;
            }
            Value = value.Equals(string.Empty) ? null : value;
        }

        public Item(object value)
        {
            Parameter = null;
            Value = value;
        }

    }
}
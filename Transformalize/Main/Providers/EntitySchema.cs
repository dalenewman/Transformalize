using System.Collections.Generic;

namespace Transformalize.Main.Providers
{
    public class EntitySchema
    {
        private List<Field> _fields = new List<Field>();

        public List<Field> Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }
    }
}
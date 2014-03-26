using System.Collections.Generic;

namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class ObjectExpression : Expression
    {
        public IEnumerable<Property> Properties;
    }
}
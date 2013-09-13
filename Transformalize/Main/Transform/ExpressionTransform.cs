#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Text;
using Transformalize.Libs.NCalc;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public class ExpressionTransform : AbstractTransform
    {
        private readonly Expression _expression;
        private readonly string _field;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public ExpressionTransform(string expression, IParameters parameters) : this(null, expression, parameters)
        {
        }

        public ExpressionTransform(string field, string expression, IParameters parameters)
            : base(parameters)
        {
            Name = "Expression";
            _field = field ?? "value";
            _expression = new Expression(expression);
            if (!_expression.HasErrors()) return;

            _log.Error("Expression '{0}' has errors: {1}", expression, _expression.Error);
            Environment.Exit(0);
        }

        public override void Transform(ref StringBuilder sb)
        {
            _expression.Parameters[_field] = sb.ToString();
            sb.Clear();
            sb.Append(_expression.Evaluate());
        }

        public override object Transform(object value)
        {
            _expression.Parameters[_field] = value;
            return _expression.Evaluate();
        }

        public override void Transform(ref Row row, string resultKey)
        {
            foreach (var pair in Parameters)
            {
                _expression.Parameters[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
            }
            row[resultKey] = _expression.Evaluate();
        }
    }
}
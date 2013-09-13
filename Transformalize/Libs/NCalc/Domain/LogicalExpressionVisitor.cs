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

namespace Transformalize.Libs.NCalc.Domain
{
    public abstract class LogicalExpressionVisitor
    {
        public abstract void Visit(LogicalExpression expression);
        public abstract void Visit(TernaryExpression expression);
        public abstract void Visit(BinaryExpression expression);
        public abstract void Visit(UnaryExpression expression);
        public abstract void Visit(ValueExpression expression);
        public abstract void Visit(Function function);
        public abstract void Visit(Identifier function);
    }
}
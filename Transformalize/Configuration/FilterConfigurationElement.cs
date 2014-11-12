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

using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Configuration {

    public class FilterConfigurationElement : ConfigurationElement {

        private const string LEFT = "left";
        private const string RIGHT = "right";
        private const string OPERATOR = "operator";
        private const string CONTINUATION = "continuation";
        private const string EXPRESSION = "expression";

        /// <summary>
        /// Optional
        /// 
        /// A reference to an entity field's name\alias or a literal on the left side of an expression.
        /// </summary>
        [ConfigurationProperty(LEFT, IsRequired = false, DefaultValue = "")]
        public string Left {
            get { return (string)this[LEFT]; }
            set { this[LEFT] = value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// A reference to an entity field's name\alias or a literal on the right side of an expression.
        /// </summary>
        [ConfigurationProperty(RIGHT, IsRequired = false, DefaultValue = "")]
        public string Right {
            get { return (string)this[RIGHT]; }
            set { this[RIGHT] = value; }
        }

        /// <summary>
        /// Optional.  Default is `Equal`
        /// 
        /// A comparison operator.  Valid values are:
        /// 
        /// * Equal
        /// * NotEqual
        /// * LessThan
        /// * LessThanEqual
        /// * GreaterThan
        /// * GreaterThanEqual 
        /// </summary>
        [EnumConversionValidator(typeof(ComparisonOperator), MessageTemplate = "{1} must be a valid ComparisonOperator. (e.g. Equal, NotEqual, LessThan, LessThanEqual, GreaterThan, GreaterThanEqual)")]
        [ConfigurationProperty(OPERATOR, IsRequired = false, DefaultValue = "Equal")]
        public string Operator {
            get { return this[OPERATOR] as string; }
            set { this[OPERATOR] = value; }
        }

        /// <summary>
        /// Optional.  Default is `And`
        /// 
        /// A continuation operator.  Valid values are:
        /// 
        /// * AND
        /// * OR
        /// </summary>
        [EnumConversionValidator(typeof(Continuation), MessageTemplate = "{1} must be a valid Continuation. (e.g. AND, OR)")]
        [ConfigurationProperty(CONTINUATION, IsRequired = false, DefaultValue = "AND")]
        public string Continuation {
            get { return this[CONTINUATION] as string; }
            set { this[CONTINUATION] = value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// A free-form, un-checked expression.  This is passed directly into a generated query.
        /// </summary>
        [ConfigurationProperty(EXPRESSION, IsRequired = false, DefaultValue = "")]
        public string Expression {
            get { return this[EXPRESSION] as string; }
            set { this[EXPRESSION] = value; }
        }
        
        public override bool IsReadOnly() {
            return false;
        }
    }
}
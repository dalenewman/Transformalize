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
    public class BranchConfigurationElement : ConfigurationElement {

        private const string NAME = "name";
        private const string TRANSFORMS = "transforms";

        //CONDITIONAL
        private const string RUN_FIELD = "run-field";
        private const string RUN_TYPE = "run-type";
        private const string RUN_OPERATOR = "run-operator";
        private const string RUN_VALUE = "run-value";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(RUN_FIELD, IsRequired = false, DefaultValue = "[default]")]
        public string RunField {
            get { return this[RUN_FIELD] as string; }
            set { this[RUN_FIELD] = value; }
        }

        [ConfigurationProperty(RUN_TYPE, IsRequired = false, DefaultValue = "[default]")]
        public string RunType {
            get { return this[RUN_TYPE] as string; }
            set { this[RUN_TYPE] = value; }
        }

        [EnumConversionValidator(typeof(ComparisonOperator), MessageTemplate = "{1} must be a valid ComparisonOperator. (e.g. Equal, NotEqual, LessThan, LessThanEqual, GreaterThan, GreaterThanEqual)")]
        [ConfigurationProperty(RUN_OPERATOR, IsRequired = false, DefaultValue = "Equal")]
        public string RunOperator {
            get { return this[RUN_OPERATOR] as string; }
            set { this[RUN_OPERATOR] = value; }
        }

        [ConfigurationProperty(RUN_VALUE, IsRequired = false, DefaultValue = "")]
        public string RunValue {
            get { return this[RUN_VALUE] as string; }
            set { this[RUN_VALUE] = value; }
        }

        [ConfigurationProperty(TRANSFORMS)]
        public TransformElementCollection Transforms {
            get { return this[TRANSFORMS] as TransformElementCollection; }
            set { this[TRANSFORMS] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }
    }
}
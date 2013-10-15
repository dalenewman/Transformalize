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
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Configuration {
    public class ValidatorConfigurationElement : ConfigurationElement {

        private const string METHOD = "method";
        private const string CHARACTERS = "characters";
        private const string SCOPE = "scope";
        private const string MESSAGE = "message";
        private const string NEGATED = "negated";
        private const string LOWER_BOUND = "lower-bound";
        private const string LOWER_BOUND_TYPE = "lower-bound-type";
        private const string UPPER_BOUND = "upper-bound";
        private const string UPPER_BOUND_TYPE = "upper-bound-type";
        
        [ConfigurationProperty(METHOD, IsRequired = true)]
        public string Method {
            get { return this[METHOD] as string; }
            set { this[METHOD] = value; }
        }

        [ConfigurationProperty(CHARACTERS, IsRequired = false, DefaultValue = "")]
        public string Characters {
            get { return this[CHARACTERS] as string; }
            set { this[CHARACTERS] = value; }
        }

        [ConfigurationProperty(MESSAGE, IsRequired = false, DefaultValue = "{1} is invalid.")]
        public string Message {
            get { return this[MESSAGE] as string; }
            set { this[MESSAGE] = value; }
        }

        [DomainValidator(new object[] { "all", "any" }, MessageTemplate = "{1} must be all, or any.")]
        [ConfigurationProperty(SCOPE, IsRequired = false, DefaultValue = "any")]
        public string Scope {
            get { return this[SCOPE] as string; }
            set { this[SCOPE] = value; }
        }

        [ConfigurationProperty(NEGATED, IsRequired = false, DefaultValue = false)]
        public bool Negated {
            get { return (bool)this[NEGATED]; }
            set { this[NEGATED] = value; }
        }

        [ConfigurationProperty(LOWER_BOUND, IsRequired = false)]
        public DateTime LowerBound {
            get { return (DateTime) this[LOWER_BOUND]; }
            set { this[LOWER_BOUND] = value; }
        }

        [ConfigurationProperty(LOWER_BOUND_TYPE, IsRequired = false, DefaultValue = "inclusive")]
        public string LowerBoundType {
            get { return this[LOWER_BOUND_TYPE] as string; }
            set { this[LOWER_BOUND_TYPE] = value; }
        }

        [ConfigurationProperty(UPPER_BOUND, IsRequired = false)]
        public DateTime UpperBound {
            get { return (DateTime)this[UPPER_BOUND]; }
            set { this[UPPER_BOUND] = value; }
        }

        [ConfigurationProperty(UPPER_BOUND_TYPE, IsRequired = false, DefaultValue = "inclusive")]
        public string UpperBoundType {
            get { return this[UPPER_BOUND_TYPE] as string; }
            set { this[UPPER_BOUND_TYPE] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }
    }
}
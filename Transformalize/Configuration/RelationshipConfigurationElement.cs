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

namespace Transformalize.Configuration {

    public class RelationshipConfigurationElement : ConfigurationElement {

        private const string LEFT_ENTITY = "left-entity";
        private const string RIGHT_ENTITY = "right-entity";
        private const string LEFT_FIELD = "left-field";
        private const string RIGHT_FIELD = "right-field";
        private const string JOIN = "join";
        private const string INDEX = "index";

        [ConfigurationProperty(LEFT_ENTITY, IsRequired = true)]
        public string LeftEntity {
            get { return this[LEFT_ENTITY] as string; }
            set { this[LEFT_ENTITY] = value; }
        }

        [ConfigurationProperty(RIGHT_ENTITY, IsRequired = true)]
        public string RightEntity {
            get { return this[RIGHT_ENTITY] as string; }
            set { this[RIGHT_ENTITY] = value; }
        }

        [ConfigurationProperty(LEFT_FIELD, IsRequired = false, DefaultValue = "")]
        public string LeftField {
            get { return this[LEFT_FIELD] as string; }
            set { this[LEFT_FIELD] = value; }
        }

        [ConfigurationProperty(RIGHT_FIELD, IsRequired = false, DefaultValue = "")]
        public string RightField {
            get { return this[RIGHT_FIELD] as string; }
            set { this[RIGHT_FIELD] = value; }
        }

        [ConfigurationProperty(INDEX, IsRequired = false, DefaultValue = false)]
        public bool Index {
            get { return (bool)this[INDEX]; }
            set { this[INDEX] = value; }
        }

        [ConfigurationProperty(JOIN)]
        public JoinElementCollection Join {
            get { return this[JOIN] as JoinElementCollection; }
        }

        public override bool IsReadOnly() {
            return false;
        }
    }
}
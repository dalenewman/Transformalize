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
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class RelationshipElementCollection : MyConfigurationElementCollection {

        private const string INDEX_MODE = "index-mode";

        public RelationshipConfigurationElement this[int index] {
            get { return BaseGet(index) as RelationshipConfigurationElement; }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        [ConfigurationProperty(INDEX_MODE, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string IndexMode {
            get { return this[INDEX_MODE] as string; }
            set { this[INDEX_MODE] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        protected override ConfigurationElement CreateNewElement() {
            return new RelationshipConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            var join = element as RelationshipConfigurationElement;
            return string.Concat(join.LeftEntity, join.RightEntity).ToLower();
        }

        public void Add(RelationshipConfigurationElement relationship) {
            BaseAdd(relationship);
        }
    }
}
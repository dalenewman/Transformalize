#region License

// /*
// Branchalize - Replicate, Branch, and Denormalize Your Data...
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

    public class BranchElementCollection : ConfigurationElementCollection {
        public BranchConfigurationElement this[int index] {
            get { return BaseGet(index) as BranchConfigurationElement; }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public override bool IsReadOnly() {
            return false;
        }

        protected override ConfigurationElement CreateNewElement() {
            return new BranchConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((BranchConfigurationElement) element).Name;
        }

        public void Add(BranchConfigurationElement element) {
            BaseAdd(element);
        }
    }
}
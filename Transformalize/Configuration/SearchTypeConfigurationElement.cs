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

namespace Transformalize.Configuration
{
    public class SearchTypeConfigurationElement : ConfigurationElement
    {
        private const string NAME = "name";
        private const string TYPE = "type";
        private const string STORE = "store";
        private const string INDEX = "index";
        private const string MULTI_VALUED = "multi-valued";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name
        {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(TYPE, IsRequired = false, DefaultValue = "inherit")]
        public string Type
        {
            get { return this[TYPE] as string; }
            set { this[TYPE] = value; }
        }

        [ConfigurationProperty(STORE, IsRequired = false, DefaultValue = true)]
        public bool Store
        {
            get { return (bool) this[STORE]; }
            set { this[STORE] = value; }
        }

        [ConfigurationProperty(INDEX, IsRequired = false, DefaultValue = true)]
        public bool Index
        {
            get { return (bool) this[INDEX]; }
            set { this[INDEX] = value; }
        }

        [ConfigurationProperty(MULTI_VALUED, IsRequired = false, DefaultValue = false)]
        public bool MultiValued
        {
            get { return (bool) this[MULTI_VALUED]; }
            set { this[MULTI_VALUED] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
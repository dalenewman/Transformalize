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
    public class JoinConfigurationElement : ConfigurationElement
    {
        private const string LEFT_FIELD = "left-field";
        private const string RIGHT_FIELD = "right-field";

        [ConfigurationProperty(LEFT_FIELD, IsRequired = true)]
        public string LeftField
        {
            get { return this[LEFT_FIELD] as string; }
            set { this[LEFT_FIELD] = value; }
        }

        [ConfigurationProperty(RIGHT_FIELD, IsRequired = true)]
        public string RightField
        {
            get { return this[RIGHT_FIELD] as string; }
            set { this[RIGHT_FIELD] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
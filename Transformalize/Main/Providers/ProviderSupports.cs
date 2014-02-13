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

namespace Transformalize.Main.Providers {
    public class ProviderSupports {
        private bool _connectionTimeout = true;
        public bool InsertMultipleRows { get; set; }
        public bool Top { get; set; }
        public bool NoLock { get; set; }
        public bool TableVariable { get; set; }
        public bool NoCount { get; set; }
        public bool MaxDop { get; set; }
        public bool IndexInclude { get; set; }
        public bool Views { get; set; }
        public bool Schemas { get; set; }

        public bool ConnectionTimeout {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value; }
        }

        
    }
}
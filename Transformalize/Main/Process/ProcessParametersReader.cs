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

namespace Transformalize.Main
{
    public class ProcessParametersReader : IParametersReader
    {
        private readonly IParameters _parameters = new Parameters();
        private readonly Process _process;

        public ProcessParametersReader(Process process)
        {
            _process = process;
        }

        public IParameters Read()
        {
            foreach (var field in _process.OutputFields().ToEnumerable())
            {
                _parameters.Add(field.Alias, field.Alias, null, field.Type);
            }
            return _parameters;
        }
    }
}
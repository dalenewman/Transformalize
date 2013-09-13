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

namespace Transformalize.Libs.FileHelpers.Helpers
{
    internal sealed class ExHelper
    {
        private ExHelper()
        {
        }

        public static void CheckNullOrEmpty(string val)
        {
            if (val == null || val.Length == 0)
                throw new ArgumentNullException("Value can´t be null or empty");
        }

        public static void CheckNullOrEmpty(string val, string paramName)
        {
            if (val == null || val.Length == 0)
                throw new ArgumentNullException(paramName, "Value can´t be null or empty");
        }

        public static void CheckNullParam(string param, string paramName)
        {
            if (param == null || param.Length == 0)
                throw new ArgumentNullException(paramName + " can´t be neither null nor empty", paramName);
        }

        public static void CheckNullParam(object param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(paramName + " can´t be null", paramName);
        }

        public static void CheckDifferentsParams(object param1, string param1Name, object param2, string param2Name)
        {
            if (param1 == param2)
                throw new ArgumentException(param1Name + " can´t be the same that " + param2Name, param1Name + " and " + param2Name);
        }

        public static void PositiveValue(int val)
        {
            if (val < 0)
                throw new ArgumentException("The value must be greater or equal than 0.");
        }
    }
}
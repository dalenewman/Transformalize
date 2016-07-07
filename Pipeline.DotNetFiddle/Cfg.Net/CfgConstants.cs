#region license
// Cfg.Net
// Copyright 2015 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cfg.Net {

    internal static class CfgConstants {

        internal static string ControlString = ((char)31).ToString();
        internal static char ControlChar = (char)31;
        internal static char NamedParameterSplitter = ':';

        internal static Dictionary<Type, Func<object, object>> Converter = new Dictionary<Type, Func<object, object>> {
                {typeof (string), (x => x)},
                {typeof (Guid), (x => Guid.Parse(x.ToString()))},
                {typeof (short), (x => Convert.ToInt16(x))},
                {typeof (int), (x => Convert.ToInt32(x))},
                {typeof (long), (x => Convert.ToInt64(x))},
                {typeof (ushort), (x => Convert.ToUInt16(x))},
                {typeof (uint), (x => Convert.ToUInt32(x))},
                {typeof (ulong), (x => Convert.ToUInt64(x))},
                {typeof (double), (x => Convert.ToDouble(x))},
                {typeof (decimal), (x => decimal.Parse(x.ToString(), NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, (IFormatProvider) CultureInfo.CurrentCulture.GetFormat(typeof (NumberFormatInfo))))},
                {typeof (char), (x => Convert.ToChar(x))},
                {typeof (DateTime), (x => Convert.ToDateTime(x))},
                {typeof (bool), (x => Convert.ToBoolean(x))},
                {typeof (float), (x => Convert.ToSingle(x))},
                {typeof (byte), (x => Convert.ToByte(x))},
                {typeof(object), (x => x)}
            };

    }
}
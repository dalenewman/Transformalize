/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Process_;

namespace Transformalize.Core
{
    public static class Common
    {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private const string APPLICATION_FOLDER = @"\Tfl\";
        private static readonly char[] Slash = new[] { '\\' };

        public static Func<KeyValuePair<string, Field>, bool> FieldFinder(ParameterConfigurationElement p)
        {
            if (p.Entity != string.Empty)
                return f => f.Key.Equals(p.Field, IC) && f.Value.Entity.Equals(p.Entity, IC) || f.Value.Name.Equals(p.Field, IC) && f.Value.Entity.Equals(p.Entity, IC);
            return f => f.Key.Equals(p.Field, IC) || f.Value.Name.Equals(p.Field, IC);
        }

        public static Func<Field, bool> FieldFinder(string nameOrAlias)
        {
            return v => v.Name.Equals(nameOrAlias, IC) || v.Alias.Equals(nameOrAlias, IC);
        }

        public static string GetTemporaryFolder()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd(Slash) + APPLICATION_FOLDER + Process.Name;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        public static IEnumerable<byte> ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            var formatter = new BinaryFormatter();
            var memory = new MemoryStream();
            formatter.Serialize(memory, obj);
            return memory.ToArray();
        }

        public static string ToSimpleType(string type)
        {
            return type.ToLower().Replace("system.", string.Empty);
        }

    }
}

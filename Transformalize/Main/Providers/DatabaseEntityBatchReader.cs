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

using Transformalize.Libs.Dapper;
using System.Linq;

namespace Transformalize.Main.Providers
{
    public class DatabaseEntityBatchReader : IEntityBatchReader
    {
        private const string SQL = "SELECT ISNULL(MAX(TflBatchId),0)+1 FROM TflBatch WHERE ProcessName = @ProcessName;";

        public int ReadNext(Process process, Entity entity)
        {
            using (var cn = process.OutputConnection.GetConnection())
            {
                cn.Open();
                return cn.Query<int>(SQL, new {ProcessName = process.Name}).First();
            }
        }
    }
}
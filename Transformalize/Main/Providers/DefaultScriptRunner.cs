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
using System.Data;

namespace Transformalize.Main.Providers
{
    public class DefaultScriptRunner : IScriptRunner
    {
        public IScriptReponse Execute(AbstractConnection connection, string script)
        {
            var response = new ScriptResponse();

            using (var cn = connection.GetConnection())
            {
                try
                {
                    cn.Open();
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = script;
                    cmd.CommandType = CommandType.Text;
                    response.RowsAffected = cmd.ExecuteNonQuery();
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.Messages.Add(e.Message);
                    if (e.InnerException != null)
                    {
                        response.Messages.Add(e.InnerException.Message);
                    }
                }
            }
            return response;
        }
    }
}
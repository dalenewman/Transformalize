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

#if ! MINI

using System.Text;

namespace Transformalize.Libs.FileHelpers.Helpers
{
    internal class DataBaseHelper
    {
        #region "  AccessConnection  "

        private const string AccessConnStr = @"Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Registry Path=;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Database Password=<PASSWORD>;Data Source=""<BASE>"";Password=;Jet OLEDB:Engine Type=5;Jet OLEDB:Global Bulk Transactions=1;Provider=""Microsoft.Jet.OLEDB.4.0"";Jet OLEDB:System database=;Jet OLEDB:SFP=False;Extended Properties=;Mode=Share Deny None;Jet OLEDB:New Database Password=;Jet OLEDB:Create System Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;User ID=Admin;Jet OLEDB:Encrypt Database=False";

        public static string GetAccessConnection(string db)
        {
            return GetAccessConnection(db, "");
        }

        public static string GetAccessConnection(string db, string password)
        {
            var res = AccessConnStr;
            res = res.Replace("<BASE>", db);

            res = res.Replace("<PASSWORD>", password == null ? string.Empty : password);
            return res;
        }

        #endregion

        #region "  SqlConnectionString  "

        private static string AppName = "FileHelpers"; //<- For display in SqlServer


        public static string SqlConnectionString(string server, string dbName)
        {
            return SqlConnectionString(server, dbName, "", "");
        }

        public static string SqlConnectionString(string server, string dbName, string user, string pass)
        {
            var sCadena = new StringBuilder(300);
            if (user.Length == 0 && pass.Length == 0)
            {
                sCadena = new StringBuilder("data source=<SERVER>;persist security info=True;" + "initial catalog=<BASE>;integrated security=SSPI;" + "packet size=4096;Connection Timeout=10;Application Name=" + AppName);
            }
            else
            {
                sCadena = new StringBuilder("data source=<SERVER>;persist security info=True;" + "initial catalog=<BASE>;User Id=\"<USER>\";Password=\"<PASS>\";" + "packet size=4096;Connection Timeout=10;Application Name=" + AppName);
                sCadena.Replace("<USER>", user);
                sCadena.Replace("<PASS>", pass);
            }
            sCadena.Replace("<SERVER>", server);
            sCadena.Replace("<BASE>", dbName);
            return sCadena.ToString();
        }

        #endregion
    }
}

#endif
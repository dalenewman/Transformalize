using System;
using System.IO;

namespace Transformalize.Providers.Json {
   public static class FileUtility {

      private static readonly char[] Slash = { '\\' };

      public static string GetTemporaryFolder(string processName) {
         var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd(Slash);

         //i.e. c: no user profile exists
         if (local.Length <= 2) {
            if (AppDomain.CurrentDomain.GetData("DataDirectory") != null) {
               local = AppDomain.CurrentDomain.GetData("DataDirectory").ToString().TrimEnd(Slash);
            }
         }

         var folder = Path.Combine(local, Constants.ApplicationFolder, processName);

         if (!Directory.Exists(folder)) {
            Directory.CreateDirectory(folder);
         }

         return folder;
      }

      public static FileInfo Find(string file) {

         if (Path.IsPathRooted(file)) {
            return new FileInfo(file);
         }

         var fileInCurrentDirectory = Path.Combine(Directory.GetCurrentDirectory(), file);
         if (File.Exists(fileInCurrentDirectory)) {
            return new FileInfo(fileInCurrentDirectory);
         }

         var fileInBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);

         return File.Exists(fileInBaseDirectory) ? new FileInfo(fileInBaseDirectory) : new FileInfo(file);
      }
   }
}